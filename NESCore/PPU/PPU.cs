﻿using Serilog;

//TODO: OAMADDR set to 0 on prerender (257-320)

namespace NESCore.PPU
{
    public class PPU : IPPU
    {
        private readonly ILogger _logger;
        private readonly IPPURegisters _registers;
        private readonly IPPUMemory _ppuMemory;

        private int _cycles = 0;
        private int _scanlines = 0;
        private int _scanlineCycles = 0;

        private byte _ppuDataBus = 0;
        private ushort _ppuAddress = 0; // v: 0 yyy NN YYYYY XXXXX 
        private ushort _tempAddress = 0; // t
        private bool _addressLatch = false;
        private byte _dataBuffer = 0;
        
        private byte _ntId = 0;
        private byte _attributeId = 0;
        private byte _patternLow = 0;
        private byte _patternHigh = 0;

        private static readonly byte[,] Palette = new byte[,]
        {
            {0x7C,0x7C,0x7C}, {0x00,0x00,0xFC}, {0x00,0x00,0xBC}, {0x44,0x28,0xBC}, {0x94,0x00,0x84}, {0xA8,0x00,0x20}, {0xA8,0x10,0x00}, {0x88,0x14,0x00},
            {0x50,0x30,0x00}, {0x00,0x78,0x00}, {0x00,0x68,0x00}, {0x00,0x58,0x00}, {0x00,0x40,0x58}, {0x00,0x00,0x00}, {0x00,0x00,0x00}, {0x00,0x00,0x00},
            {0xBC,0xBC,0xBC}, {0x00,0x78,0xF8}, {0x00,0x58,0xF8}, {0x68,0x44,0xFC}, {0xD8,0x00,0xCC}, {0xE4,0x00,0x58}, {0xF8,0x38,0x00}, {0xE4,0x5C,0x10},
            {0xAC,0x7C,0x00}, {0x00,0xB8,0x00}, {0x00,0xA8,0x00}, {0x00,0xA8,0x44}, {0x00,0x88,0x88}, {0x00,0x00,0x00}, {0x00,0x00,0x00}, {0x00,0x00,0x00},
            {0xF8,0xF8,0xF8}, {0x3C,0xBC,0xFC}, {0x68,0xA0,0xFC}, {0xB0,0x7C,0xFC}, {0xFA,0x78,0xF8}, {0xF8,0x78,0x98}, {0xF8,0x88,0x58}, {0xF8,0xA8,0x44},
            {0xF8,0xD8,0x78}, {0x98,0xF8,0x58}, {0x54,0xF8,0x58}, {0x58,0xF8,0x98}, {0x44,0xD8,0xD8}, {0x78,0x78,0x78}, {0x00,0x00,0x00}, {0x00,0x00,0x00},
            {0xFC,0xFC,0xFC}, {0xA4,0xE4,0xFC}, {0xB8,0xB8,0xF8}, {0xD8,0xB8,0xF8}, {0xF8,0xB8,0xF8}, {0xF8,0xA4,0xC0}, {0xF0,0xD0,0xB0}, {0xFC,0xE4,0xA0},
            {0xE4,0xF4,0xB0}, {0xB4,0xFA,0xB4}, {0xB4,0xF4,0xD8}, {0x00,0xFA,0xFA}, {0xCC,0xCC,0xCC}, {0x00,0x00,0x00}, {0x00,0x00,0x00}, {0x00,0x00,0x00}
        };
        

        const ushort CoarseYMask = 0x03E0;

        private readonly byte[] _frameBuffer = new byte[256 * 240 * 3];

        public event NMIEventHandler? NMIEvent;

        public PPU(IPPURegisters registers, IPPUMemory ppuMemory, ILogger logger)
        {
            _logger = logger;
            _registers = registers;
            _ppuMemory = ppuMemory;

            Array.Fill(_frameBuffer, (byte)0);
        }

        public byte Read(ushort address)
        {
            address = (ushort)(0x2000 + (address & 0x0007));
            
            //TODO: replace this branching with an indexed array (_ppuMemory[address & 0x7])
            if (address == 0x2000)
            {
                _logger.Warning("PPU control register read 0x2000. Normally this should not happen.");
                return _ppuDataBus;
            }
            else if (address == 0x2001)
            {
                _logger.Warning("PPU mask register read 0x2001. Normally this should not happen.");
                return _ppuDataBus;
            }
            else if (address == 0x2002)
            {
                _ppuDataBus = (byte)((_registers.Status & 0xE0) | (_ppuDataBus & 0x1F));
                _registers.SetVBlankFlag(false);
                _addressLatch = false;
                return _ppuDataBus;
            }
            else if(address == 0x2003)
            {
                _logger.Warning("PPU OAM address register read 0x2003. Normally this should not happen.");
                return _ppuDataBus;
            }
            else if(address == 0x2004)
            {
                _ppuDataBus = _ppuMemory.OamRead(_registers.OAMAddress);
                return _ppuDataBus;
            }
            else if(address == 0x2005)
            {
                _logger.Warning("PPU scroll register read 0x2005. Normally this should not happen.");
                return _ppuDataBus;
            }
            else if (address == 0x2006)
            {
                _logger.Warning("PPU address register read 0x2006. Normally this should not happen.");
                return _ppuDataBus;
            }
            else if(address == 0x2007)
            {
                if (_ppuAddress < 0x3f00)
                {
                    var result = _dataBuffer;
                    _dataBuffer = _ppuMemory.Read(_ppuAddress);
                    _ppuAddress = (ushort)((_ppuAddress + _registers.VRAMIncrement()) & 0x3FFF);
                    _ppuDataBus = result;
                    return result;
                }
                else
                {
                    var result = _ppuMemory.Read(_ppuAddress);
                    _dataBuffer = _ppuMemory.Read((ushort)(_ppuAddress - 0x1000));
                    _ppuAddress = (ushort)((_ppuAddress + _registers.VRAMIncrement()) & 0x3FFF);
                    _ppuDataBus = result;
                    return result;
                }
            }

            _logger.Error($"Unsupported PPU read from: 0x{address:X4}");
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
            
            if (address == 0x2000)
            {
                _registers.Control = value;
                _tempAddress = (ushort)((_tempAddress & 0xF3FF) 
                                           | (value & 0x03) << 10);
            }
            else if(address == 0x2001)
            {
                _registers.Mask = value;
            }
            else if(address == 0x2003)
            {
                _registers.OAMAddress = value;
            }
            else if(address == 0x2004)
            {
                _ppuMemory.OamWrite(_registers.OAMAddress, value);
                _registers.OAMAddress++;
            }
            else if(address == 0x2005)
            {
                _registers.Scroll = value;
                if(_addressLatch == false)
                {
                    _registers.XScroll = value;
                    _registers.FineX = (byte)(value & 0x07);
                    _registers.CoarseX = (ushort)((value) >> 3);
                    _tempAddress = (ushort)((_tempAddress & 0xffe0) | _registers.CoarseX);
                }
                else
                {
                    _registers.YScroll = value;
                    //FGH..AB CDE..... <- d: ABCDEFGH
                    _registers.FineY = (ushort)(value & 0x07);
                    _registers.CoarseY = (ushort)(value >> 3);
                    _tempAddress = (ushort)((_tempAddress & 0x8C1F) // mask out fineY and coarseY
                                               | (_registers.FineY << 12) 
                                               | (_registers.CoarseY << 5));
                }
                _addressLatch = !_addressLatch;
            }
            else if(address == 0x2006)
            {
                _registers.Address = value;
                if(_addressLatch == false)
                {
                    _tempAddress = (ushort)( (_tempAddress & 0x00ff) | ((value & 0x3f) << 8));
                    _addressLatch = true;
                }
                else
                {
                    _tempAddress = (ushort)((_tempAddress & 0xff00) | value);
                    _ppuAddress = _tempAddress;
                    _addressLatch = false;
                }
            }
            else if(address == 0x2007)
            {
                _ppuMemory.Write(_ppuAddress, value);
                _ppuAddress = (ushort)((_ppuAddress + _registers.VRAMIncrement()) & 0x3fff);
            }
            else
            {

                _logger.Error($"Unsupported PPU write to: 0x{address:X4}");
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

        private ushort GetCurrentPixelPaletteAddress()
        {
            var bitPosition = 15 - _registers.FineX;

            var colorIndex = (((_shiftRegisterLo >> bitPosition) & 0x01) | 
                              (((_shiftRegisterHi >> bitPosition) & 0x01) << 1));
    
            if (colorIndex == 0)
            {
                return 0x3F00;
            }
    
            var paletteId = (((_attributeRegisterLo >> bitPosition) & 0x01) | 
                             (((_attributeRegisterHi >> bitPosition) & 0x01) << 1));
    
            return (ushort)(0x3F00 | (paletteId << 2) | colorIndex);
        }
        
        internal ushort GetCurrentPixelPaletteAddressTest() => GetCurrentPixelPaletteAddress();
        
        private void IncrementCoarseX()
        {
            // From https://www.nesdev.org/wiki/PPU_scrolling
            // wrap around:
            if ((_ppuAddress & 0b11111) == 31)
            {
                _ppuAddress &= 0xFFE0; // zero out the bottom 5 bits (coarse x part of the address)
                _ppuAddress ^= 0x0400; // Switch to next nametable

                _registers.CoarseX = 0;
            }
            else
            {
                _ppuAddress = (ushort)(_ppuAddress + 1);
                _registers.CoarseX++;
            }
            
        }

        private void IncrementY()
        {
            // increment fineY at the end of the scanline
            var fineY = (ushort)((_ppuAddress >> 12) & 0x07);
            if (fineY < 7) 
            {
                _ppuAddress += 0x1000; // increment fineY
            }
            else
            {
                var coarseY = (ushort)((_ppuAddress & CoarseYMask) >> 5);
                _ppuAddress &= 0x8FFF;

                if (coarseY == 29)
                {
                    // end of the nametable => change NT
                    // coarseY = 0;
                    // NT++
                    coarseY = 0;
                    _ppuAddress ^= 0x0800;
                }
                else if (coarseY == 31)
                {
                    coarseY = 0;
                }
                else
                {
                    coarseY++;
                }
                             
                _ppuAddress = (ushort)((_ppuAddress & ~CoarseYMask) | (coarseY << 5));
            }
        }
        
        private ushort _shiftRegisterHi = 0;
        private ushort _shiftRegisterLo = 0;
        private ushort _attributeRegisterLo = 0;
        private ushort _attributeRegisterHi = 0;
        
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
                    // Only output pixels to the framebuffer on visible scanlines (0..239)
                    if (isRenderScanline && _scanlineCycles < 256)
                    {
                        var paletteAddress = GetCurrentPixelPaletteAddress();
                        var paletteId = _ppuMemory.Read(paletteAddress) & 0x3F;
                        var pxOffset = (_scanlineCycles + _scanlines * 256) * 3;
                        _frameBuffer[pxOffset + 2] = Palette[paletteId, 0];
                        _frameBuffer[pxOffset + 1] = Palette[paletteId, 1];
                        _frameBuffer[pxOffset + 0] = Palette[paletteId, 2];
                    }

                    MoveShiftRegisters();

                    var phase = _scanlineCycles % 8;
                    if (phase == 1)
                    {
                        var ntAddress = (ushort)(0x2000 | (_ppuAddress & 0x0FFF));
                        _ntId = _ppuMemory.Read(ntAddress);
                    }
                    else if (phase == 3)
                    {
                        // Fetch attribute table
                        // attribute address = 0x23C0 | (v & 0x0C00) | ((v >> 4) & 0x38) | ((v >> 2) & 0x07)
                        var atAddress = (ushort)(0x23C0 | (_ppuAddress & 0x0C00) | ((_ppuAddress >> 4) & 0x0038) |
                                                           ((_ppuAddress >> 2) & 0x0007));
                        _attributeId = _ppuMemory.Read(atAddress);
                    }
                    else if (phase == 5)
                    {
                        // pattern table low
                        // address is based on the control register
                        var patternBaseAddress = _registers.GetbackgroundPatternTableAddress();
                        var fineY = (ushort)((_ppuAddress >> 12) & 0x07);
                        var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16);
                        _patternLow = _ppuMemory.Read(ptAddress);
                    }
                    else if (phase == 7)
                    {
                        // pattern table low
                        // address is based on the control register
                        var patternBaseAddress = _registers.GetbackgroundPatternTableAddress();
                        var fineY = (ushort)((_ppuAddress >> 12) & 0x07);
                        var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16 + 8);
                        _patternHigh = _ppuMemory.Read(ptAddress);

                        // Reload shift registers (lower 8 bits)
                        _shiftRegisterLo = (ushort)((_shiftRegisterLo & 0xFF00) | _patternLow);
                        _shiftRegisterHi = (ushort)((_shiftRegisterHi & 0xFF00) | _patternHigh);

                        // Reload attribute registers (lower 8 bits with 8 duplicated bits)
                        var coarseXBit1 = (_ppuAddress & 0x0002);
                        var coarseYBit1 = (_ppuAddress & 0x0040) >> 4;
                        var shiftAmount = coarseYBit1 | coarseXBit1;

                        var paletteBits = (_attributeId >> shiftAmount) & 0x03;
                        var attrLo = (byte)((paletteBits & 1) != 0 ? 0xFF : 0x00);
                        var attrHi = (byte)(((paletteBits >> 1) & 1) != 0 ? 0xFF : 0x00);
                        _attributeRegisterLo = (ushort)((_attributeRegisterLo & 0xFF00) | attrLo);
                        _attributeRegisterHi = (ushort)((_attributeRegisterHi & 0xFF00) | attrHi);

                        IncrementCoarseX();
                    }

                    if (_scanlineCycles == 255)
                    {
                        IncrementY();
                    }
                }
                else if (renderingEnabled && _scanlineCycles == 256)
                {
                    // Reset horizontal scroll (coarse X and NT horizontal bit)
                    _ppuAddress = (ushort)((_ppuAddress & ~0x041F) | (_tempAddress & 0x041F));
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
                        // Reset vertical scroll
                        _ppuAddress = (ushort)((_ppuAddress & ~0x7BE0) | (_tempAddress & 0x7BE0));
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
