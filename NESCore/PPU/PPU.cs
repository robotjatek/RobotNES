using Serilog;

namespace NESCore.PPU
{
    public class Ppu : IPPU
    {
        private const int VBlankScanline = 241;
        private const int PreRenderScanline = 261;
        private const int VBlankSetDot = 1;
        private const int VBlankClearDot = 2;
        
        private readonly ILogger _logger;
        private readonly IPPURegisters _registers;
        private readonly IPPUMemory _ppuMemory;
        private readonly LoopyRegisters _loopyRegisters = new();
        private readonly SpriteEvaluator _spriteEvaluator;
        private readonly BackgroundEvaluator _backgroundEvaluator;
        internal LoopyRegisters Loopy => _loopyRegisters;

        // Test-only hook for render tests comparing the framebuffer output (see InternalsVisibleTo).
        internal byte[] FrameBuffer => _frameBuffer;
        private int _cycles;
        private int _scanlines;
        private int _scanlineCycles;

        private byte _ppuDataBus;
        private byte _dataBuffer;
        private bool _oddFrame;
        private bool _suppressVBlankThisFrame;

        public readonly record struct Color(byte R, byte G, byte B);

        public static readonly Color[] Palette =
        [
            new(0x7C, 0x7C, 0x7C), new(0x00, 0x00, 0xFC), new(0x00, 0x00, 0xBC), new(0x44, 0x28, 0xBC),
            new(0x94, 0x00, 0x84), new(0xA8, 0x00, 0x20), new(0xA8, 0x10, 0x00), new(0x88, 0x14, 0x00),
            new(0x50, 0x30, 0x00), new(0x00, 0x78, 0x00), new(0x00, 0x68, 0x00), new(0x00, 0x58, 0x00),
            new(0x00, 0x40, 0x58), new(0x00, 0x00, 0x00), new(0x00, 0x00, 0x00), new(0x00, 0x00, 0x00),
            new(0xBC, 0xBC, 0xBC), new(0x00, 0x78, 0xF8), new(0x00, 0x58, 0xF8), new(0x68, 0x44, 0xFC),
            new(0xD8, 0x00, 0xCC), new(0xE4, 0x00, 0x58), new(0xF8, 0x38, 0x00), new(0xE4, 0x5C, 0x10),
            new(0xAC, 0x7C, 0x00), new(0x00, 0xB8, 0x00), new(0x00, 0xA8, 0x00), new(0x00, 0xA8, 0x44),
            new(0x00, 0x88, 0x88), new(0x00, 0x00, 0x00), new(0x00, 0x00, 0x00), new(0x00, 0x00, 0x00),
            new(0xF8, 0xF8, 0xF8), new(0x3C, 0xBC, 0xFC), new(0x68, 0xA0, 0xFC), new(0xB0, 0x7C, 0xFC),
            new(0xFA, 0x78, 0xF8), new(0xF8, 0x78, 0x98), new(0xF8, 0x88, 0x58), new(0xF8, 0xA8, 0x44),
            new(0xF8, 0xD8, 0x78), new(0x98, 0xF8, 0x58), new(0x54, 0xF8, 0x58), new(0x58, 0xF8, 0x98),
            new(0x44, 0xD8, 0xD8), new(0x78, 0x78, 0x78), new(0x00, 0x00, 0x00), new(0x00, 0x00, 0x00),
            new(0xFC, 0xFC, 0xFC), new(0xA4, 0xE4, 0xFC), new(0xB8, 0xB8, 0xF8), new(0xD8, 0xB8, 0xF8),
            new(0xF8, 0xB8, 0xF8), new(0xF8, 0xA4, 0xC0), new(0xF0, 0xD0, 0xB0), new(0xFC, 0xE4, 0xA0),
            new(0xE4, 0xF4, 0xB0), new(0xB4, 0xFA, 0xB4), new(0xB4, 0xF4, 0xD8), new(0x00, 0xFA, 0xFA),
            new(0xCC, 0xCC, 0xCC), new(0x00, 0x00, 0x00), new(0x00, 0x00, 0x00), new(0x00, 0x00, 0x00)
        ];

        private readonly byte[] _frameBuffer = new byte[256 * 240 * 3];

        public event VBlankEventHandler? VBlank;

        public Ppu(IPPURegisters registers, IPPUMemory ppuMemory, ILogger logger)
        {
            _logger = logger;
            _registers = registers;
            _ppuMemory = ppuMemory;
            _backgroundEvaluator = new BackgroundEvaluator(_loopyRegisters, _ppuMemory, _registers);
            _spriteEvaluator = new SpriteEvaluator(registers, ppuMemory);
            Array.Fill(_frameBuffer, (byte)0);
        }
        
        public void BeginStatusRead()
        {
            // Hardware quirk (2C02): reading $2002 on the exact dot before VBlank starts (241:0)
            // clears the VBlank latch simultaneously with its set signal, suppressing VBlank for this frame.
            if (_scanlines == 241 && _scanlineCycles == 0)
            {
                _suppressVBlankThisFrame = true;
            }
        }

        public byte Read(ushort address)
        {
            address = (ushort)(0x2000 + (address & 0x0007));

            //TODO: replace this branching with an indexed array (_ppuMemory[address & 0x7])
            if (address == 0x2000) // control
            {
                _logger.Warning("PPU control register read 0x2000. Normally this should not happen");
                return _ppuDataBus;
            }
            else if (address == 0x2001)
            {
                _logger.Warning("PPU mask register read 0x2001. Normally this should not happen");
                return _ppuDataBus;
            }
            else if (address == 0x2002) // status
            {
                _ppuDataBus = (byte)((_registers.Status & 0xE0) | (_ppuDataBus & 0x1F));
                _registers.SetVBlankFlag(false);
                _loopyRegisters.ReadStatus();
                return _ppuDataBus;
            }
            else if (address == 0x2003)
            {
                _logger.Warning("PPU OAM address register read 0x2003. Normally this should not happen");
                return _ppuDataBus;
            }
            else if (address == 0x2004)
            {
                _ppuDataBus = _ppuMemory.OamRead(_registers.OAMAddress);
                return _ppuDataBus;
            }
            else if (address == 0x2005)
            {
                _logger.Warning("PPU scroll register read 0x2005. Normally this should not happen");
                return _ppuDataBus;
            }
            else if (address == 0x2006)
            {
                _logger.Warning("PPU address register read 0x2006. Normally this should not happen");
                return _ppuDataBus;
            }
            else if (address == 0x2007)
            {
                if (!_loopyRegisters.IsPaletteAccess())
                {
                    var result = _dataBuffer;
                    _dataBuffer = _ppuMemory.Read(_loopyRegisters.V);
                    _loopyRegisters.IncrementAddress(_registers);
                    _ppuDataBus = result;
                    return result;
                }
                else // palette read
                {
                    // palette is just 6 bits, the top two bits are openbus
                    var grayscaleMask = _registers.IsGrayScale() ? 0x30 : 0xFF;
                    var result = (byte)(((_ppuMemory.Read(_loopyRegisters.V) & 0x3f) | _ppuDataBus & 0xC0) & grayscaleMask);
                    _dataBuffer = _ppuMemory.Read(_loopyRegisters.GetPaletteMirrorAddress());
                    _loopyRegisters.IncrementAddress(_registers);
                    _ppuDataBus = result;
                    return result;
                }
            }

            _logger.Error("Unsupported PPU read from: 0x{Address:X4}", address);
            throw new NotImplementedException();
        }

        public void OamDMAWrite(byte value)
        {
            _ppuMemory.OamWrite(_registers.OAMAddress, value);
            _registers.OAMAddress++;
        }

        public void Write(ushort address, byte value)
        {
            address = (ushort)(0x2000 + (address & 0x0007));
            _ppuDataBus = value;

            if (address == 0x2000) // Control register
            {
                _registers.Control = value;
                _loopyRegisters.WriteControl(value);
            }
            else if (address == 0x2001)
            {
                _registers.Mask = value;
            }
            else if (address == 0x2002)
            {
                // Read-only register: NO-OP
            }
            else if (address == 0x2003)
            {
                _registers.OAMAddress = value;
            }
            else if (address == 0x2004)
            {
                _ppuMemory.OamWrite(_registers.OAMAddress, value);
                _registers.OAMAddress++;
            }
            else if (address == 0x2005)
            {
                _loopyRegisters.ScrollWrite(value);
            }
            else if (address == 0x2006)
            {
                _loopyRegisters.WriteAddress(value);
            }
            else if (address == 0x2007)
            {
                _ppuMemory.Write(_loopyRegisters.V, value);
                _loopyRegisters.IncrementAddress(_registers);
            }
            else
            {
                _logger.Error("Unsupported PPU write to: 0x{Address:X4}", address);
                throw new NotImplementedException();
            }
        }

        public void Run(uint cycles)
        {
            for (var i = 0; i < cycles; i++)
            {
                Cycle();
            }
        }

        private void Cycle()
        {
            var renderingEnabled = _registers.ShowBackground() || _registers.ShowSprites();
            var isRenderScanline = _scanlines is >= 0 and < 240;
            var isPreRenderScanline = _scanlines == PreRenderScanline;

            if (isRenderScanline || isPreRenderScanline)
            {
                if (renderingEnabled && _scanlineCycles is < 256 or >= 320 and < 336)
                {
                    if (_scanlineCycles == 0)
                    {
                        _spriteEvaluator.EvaluateSprites(_scanlines);
                    }

                    // Only output pixels to the framebuffer on visible scanlines (0..239)
                    if (isRenderScanline && _scanlineCycles < 256)
                    {
                        var pxOffset = (_scanlineCycles + _scanlines * 256) * 3;

                        var showBg = _registers.ShowBackground() &&
                                     (_scanlineCycles >= 8 || _registers.ShowBackgroundInLeftColumn());
                        var showSpr = _registers.ShowSprites() &&
                                      (_scanlineCycles >= 8 || _registers.ShowSpritesInLeftColumn());
                        var bgPx = _backgroundEvaluator.NextPixel(_scanlineCycles);

                        var sprData = _spriteEvaluator.SpriteScanlineBuffer[_scanlineCycles];
                        var sprOpaque = showSpr && sprData.PixelColor is not null;

                        Color pixel;
                        if (sprOpaque && (!bgPx.opaque || !sprData.BehindBackground))
                        {
                            pixel = sprData.PixelColor!.Value;
                        }
                        else
                        {
                            pixel = bgPx.pixel;
                        }

                        // Sprite 0 Hit: both pixels must be opaque
                        if (showBg && showSpr &&
                            sprData is { PixelColor: not null, Zero: true } &&
                            bgPx.opaque &&
                            _scanlineCycles < 255)
                        {
                            _registers.SetSpriteZeroHitFlag(true);
                        }

                        _frameBuffer[pxOffset + 2] = pixel.R;
                        _frameBuffer[pxOffset + 1] = pixel.G;
                        _frameBuffer[pxOffset + 0] = pixel.B;
                    }

                    _backgroundEvaluator.Tick(_scanlineCycles);
                }
                else if (renderingEnabled && _scanlineCycles == 256)
                {
                    // Reset horizontal scroll (coarse X and NT horizontal bit)
                    _loopyRegisters.ResetHorizontalScroll();
                }

                // Pre-render line specific actions
                if (isPreRenderScanline)
                {
                    if (renderingEnabled && _scanlineCycles is >= 257 and <= 320)
                    {
                        _registers.OAMAddress = 0;
                    }

                    if (_scanlineCycles == VBlankClearDot)
                    {
                        _registers.SetVBlankFlag(false);
                        _registers.SetOverflowFlag(false);
                        _registers.SetSpriteZeroHitFlag(false);
                    }

                    if (_scanlineCycles >= 280 && _scanlineCycles <= 304 && renderingEnabled)
                    {
                        _loopyRegisters.ResetVerticalScroll();
                    }
                }
            }
            else if (_scanlines == 240)
            {
                // Post-render scanline: idle
            }
            else if (_scanlines >= VBlankScanline && _scanlines <= 260)
            {
                // VBlank scanlines
                if (_scanlines == VBlankScanline && _scanlineCycles == VBlankSetDot)
                {
                    if (!_suppressVBlankThisFrame)
                    {
                        _registers.SetVBlankFlag(true);
                    }
                    _suppressVBlankThisFrame = false;

                    // The rendered frame is handed over on every VBlank. The NMI enable bit in PPU
                    // control only decides whether the CPU gets interrupted, so ROMs that never set
                    // it (e.g. the sprite hit / frame basics test ROMs) still get displayed.
                    VBlank?.Invoke(_frameBuffer);
                }
            }

            _cycles++;
            _scanlineCycles++;
            if (_scanlines == PreRenderScanline && _scanlineCycles == 340 && _oddFrame && renderingEnabled)
            {
                _scanlineCycles = 341;
            }
            if (_scanlineCycles == 341)
            {
                _scanlines++;
                _scanlineCycles = 0;
                if (_scanlines > PreRenderScanline)
                {
                    _scanlines = 0;
                    _oddFrame = !_oddFrame;
                }
            }
        }
        
        public bool NmiOutput => _registers.GetVBlankFlag() && _registers.GetEnableNMI();
    }
}