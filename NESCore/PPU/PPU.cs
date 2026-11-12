using Serilog;

//TODO: OAMADDR set to 0 on prerender (257-320)

namespace NESCore.PPU
{
    public class Ppu : IPPU
    {
        private readonly ILogger _logger;
        private readonly IPPURegisters _registers;
        private readonly IPPUMemory _ppuMemory;
        private readonly LoopyRegisters _loopyRegisters = new LoopyRegisters();
        private readonly SpriteEvaluator _spriteEvaluator;

        private int _cycles;
        private int _scanlines;
        private int _scanlineCycles;

        private byte _ppuDataBus;
        private byte _dataBuffer;

        private byte _ntId;
        private byte _attributeId;
        private byte _patternLow;
        private byte _patternHigh;
        
        private ushort _shiftRegisterHi;
        private ushort _shiftRegisterLo;
        private ushort _attributeRegisterLo;
        private ushort _attributeRegisterHi;

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

        public event NMIEventHandler? NMIEvent;

        public Ppu(IPPURegisters registers, IPPUMemory ppuMemory, ILogger logger)
        {
            _logger = logger;
            _registers = registers;
            _ppuMemory = ppuMemory;
            _spriteEvaluator = new SpriteEvaluator(registers, ppuMemory);

            Array.Fill(_frameBuffer, (byte)0);
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
                else
                {
                    var result = _ppuMemory.Read(_loopyRegisters.V);
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

        // The shift registers hold the next 16 pixels, so fine x indexes them from the top down
        private int CurrentPixelBitPosition => 15 - _loopyRegisters.FineX;

        private ushort GetCurrentPixelPaletteAddress()
        {
            var bitPosition = CurrentPixelBitPosition;
            var colorIndex = GetBgColorPaletteId();

            if (colorIndex == 0)
            {
                return 0x3F00;
            }

            var paletteId = (((_attributeRegisterLo >> bitPosition) & 0x01) |
                             (((_attributeRegisterHi >> bitPosition) & 0x01) << 1));

            return (ushort)(0x3F00 | (paletteId << 2) | colorIndex);
        }
        
        private ushort GetBgColorPaletteId()
        {
            var bitPosition = CurrentPixelBitPosition;

            var colorIndex = ((_shiftRegisterLo >> bitPosition) & 0x01) |
                             (((_shiftRegisterHi >> bitPosition) & 0x01) << 1);

            return (ushort)(colorIndex);
        }

        internal ushort GetCurrentPixelPaletteAddressTest() => GetCurrentPixelPaletteAddress();

        internal LoopyRegisters Loopy => _loopyRegisters;

        private void MoveShiftRegisters()
        {
            _shiftRegisterLo <<= 1;
            _shiftRegisterHi <<= 1;

            _attributeRegisterLo <<= 1;
            _attributeRegisterHi <<= 1;
        }

        private void Cycle()
        {
            var renderingEnabled = _registers.ShowBackground() || _registers.ShowSprites();
            var isRenderScanline = _scanlines is >= 0 and < 240;
            var isPreRenderScanline = _scanlines == 261;

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
                        var paletteAddress = GetCurrentPixelPaletteAddress();
                        var paletteId = _ppuMemory.Read(paletteAddress) & 0x3F;
                        var pxOffset = (_scanlineCycles + _scanlines * 256) * 3;

                        var showBg = _registers.ShowBackground() &&
                                     (_scanlineCycles >= 8 || _registers.ShowBackgroundInLeftColumn());
                        var showSpr = _registers.ShowSprites() &&
                                      (_scanlineCycles >= 8 || _registers.ShowSpritesInLeftColumn());
                        var bgPx = showBg
                            ? Palette[paletteId]
                            : Palette[_ppuMemory.Read(0x3F00)]; // Render the default background color
                        var bgOpaque = showBg && GetBgColorPaletteId() != 0;
                        var sprData = _spriteEvaluator.SpriteScanlineBuffer[_scanlineCycles];
                        var sprOpaque = showSpr && sprData.PixelColor is not null;

                        Color pixel;
                        if (sprOpaque && (!bgOpaque || !sprData.BehindBackground))
                        {
                            pixel = sprData.PixelColor!.Value;
                        }
                        else
                        {
                            pixel = bgPx;
                        }
                        
                        // Sprite 0 Hit: both pixels must be opaque
                        if (showBg && showSpr && 
                            sprData is { PixelColor: not null, Zero: true } &&
                            GetBgColorPaletteId() != 0 &&
                            _scanlineCycles < 255)
                        {
                            _registers.SetSpriteZeroHitFlag(true);
                        }
                        
                        _frameBuffer[pxOffset + 2] = pixel.R;
                        _frameBuffer[pxOffset + 1] = pixel.G;
                        _frameBuffer[pxOffset + 0] = pixel.B;
                    }

                    MoveShiftRegisters();

                    var phase = _scanlineCycles % 8;
                    if (phase == 1)
                    {
                        var ntAddress = _loopyRegisters.GetNametableAddress();
                        _ntId = _ppuMemory.Read(ntAddress);
                    }
                    else if (phase == 3)
                    {
                        var atAddress = _loopyRegisters.GetAttributeAddress();
                        _attributeId = _ppuMemory.Read(atAddress);
                    }
                    else if (phase == 5)
                    {
                        // pattern table low
                        // address is based on the control register
                        var patternBaseAddress = _registers.GetbackgroundPatternTableAddress();
                        var fineY = (ushort)_loopyRegisters.GetFineYScroll();
                        var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16);
                        _patternLow = _ppuMemory.Read(ptAddress);
                    }
                    else if (phase == 7)
                    {
                        // pattern table low
                        // address is based on the control register
                        var patternBaseAddress = _registers.GetbackgroundPatternTableAddress();
                        var fineY = (ushort)_loopyRegisters.GetFineYScroll();
                        var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16 + 8);
                        _patternHigh = _ppuMemory.Read(ptAddress);

                        // Reload shift registers (lower 8 bits)
                        _shiftRegisterLo = (ushort)((_shiftRegisterLo & 0xFF00) | _patternLow);
                        _shiftRegisterHi = (ushort)((_shiftRegisterHi & 0xFF00) | _patternHigh);
                        
                        var shiftAmount = _loopyRegisters.GetAttributeShift();

                        var paletteBits = (_attributeId >> shiftAmount) & 0x03;
                        var attrLo = (byte)((paletteBits & 1) != 0 ? 0xFF : 0x00);
                        var attrHi = (byte)(((paletteBits >> 1) & 1) != 0 ? 0xFF : 0x00);
                        _attributeRegisterLo = (ushort)((_attributeRegisterLo & 0xFF00) | attrLo);
                        _attributeRegisterHi = (ushort)((_attributeRegisterHi & 0xFF00) | attrHi);

                        _loopyRegisters.IncrementCoarseX();
                    }

                    if (_scanlineCycles == 255)
                    {
                        _loopyRegisters.IncrementY();
                    }
                }
                else if (renderingEnabled && _scanlineCycles == 256)
                {
                    // Reset horizontal scroll (coarse X and NT horizontal bit)
                    _loopyRegisters.ResetHorizontalScroll();
                }

                // Pre-render line specific actions
                if (isPreRenderScanline)
                {
                    if (_scanlineCycles == 1)
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
            else if (_scanlines >= 241 && _scanlines <= 260)
            {
                // VBlank scanlines
                if (_scanlines == 241 && _scanlineCycles == 0)
                {
                    _registers.SetVBlankFlag(true);
                    if (_registers.GetEnableNMI())
                    {
                        NMIEvent?.Invoke(_frameBuffer);
                    }
                }
            }

            _cycles++;
            _scanlineCycles++;
            if (_scanlineCycles == 341)
            {
                _scanlines++;
                _scanlineCycles = 0;
                if (_scanlines > 261)
                {
                    _scanlines = 0;
                }
            }
        }
    }
}
