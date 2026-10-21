using Serilog;

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
        private ushort _ppuAddress = 0; // v
        private ushort _tempPPUAddress = 0; // t
        private bool _addressLatch = false;
        private byte _dataBuffer = 0;

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
                }
                else
                {
                    _registers.YScroll = value;
                }
                _addressLatch = !_addressLatch;
            }
            else if(address == 0x2006)
            {
                _registers.Address = value;
                if(_addressLatch == false)
                {
                    _tempPPUAddress = (ushort)( (_tempPPUAddress & 0x00ff) | ((value & 0x3f) << 8));
                    _addressLatch = true;
                }
                else
                {
                    _tempPPUAddress = (ushort)((_tempPPUAddress & 0xff00) | value);
                    _ppuAddress = _tempPPUAddress;
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

        Random r = new ();

        private void Cycle()
        {
            if (_scanlines >= 0 && _scanlines < 240)
            {
                // TODO: fetch AT (3-4)
                // TODO: fetch pattern LS (5-6)
                // TODO: fetch pattern MS (7-8)
                // TODO: increment horiV (8)
                
                //Cycle visible scanline
                if (_scanlineCycles < 256)
                {
                    var phase = _scanlineCycles % 8;
                    if (phase == 1)
                    {
                        // Fetch nametable:
                        // tile address = 0x2000 | (v & 0x0FFF)
                        
                        var ntAddress = (ushort)(0x2000 | (_ppuAddress & 0x0fff));
                        var nt = _ppuMemory.Read(ntAddress); // TODO: save this as member
                    } 
                    else if (phase == 3)
                    {
                        // Fetch attribute table
                        // attribute address = 0x23C0 | (v & 0x0C00) | ((v >> 4) & 0x38) | ((v >> 2) & 0x07)
                        var atAddress = (ushort)(0x23C0 | (_ppuAddress & 0x0C00) | ((_ppuAddress >> 4) & 0x0038) |
                                                           ((_ppuAddress >> 2) & 0x0007));
                        var at = _ppuMemory.Read(atAddress);
                    }
                }
                
                
                
                if (_scanlineCycles < 256)
                {
                    var pixel = (_scanlineCycles + _scanlines * 256) * 3;
                    _frameBuffer[pixel] = (byte)r.Next(0, 255);
                    _frameBuffer[pixel + 1] = (byte)r.Next(0, 255);
                    _frameBuffer[pixel + 2] = (byte)r.Next(0, 255);
                }
            }
            else if (_scanlines == 240)
            {
                //cycle post render scanline
            }
            else if(_scanlines >= 241 && _scanlines <= 260)
            {
                //cycle vblank scanline
                if(_scanlines == 241 && _scanlineCycles == 1)
                {
                    _registers.SetVBlankFlag(true);
                    if (_registers.GetEnableNMI())
                    {
                        NMIEvent?.Invoke(_frameBuffer);
                    }
                }
            }
            else if(_scanlines == 261)
            {
                //cycle prerender scanline

                if (_scanlineCycles == 1)
                {
                    _registers.SetVBlankFlag(false);
                    _registers.SetOverflowFlag(false);
                    _registers.SetSpriteZeroHitFlag(false);
                }
            }

            _cycles++;
            _scanlineCycles++;
            if(_scanlineCycles == 341)
            {
                _scanlines++;
                _scanlineCycles = 0;
                if(_scanlines > 261)
                {
                    _scanlines = 0;
                }
            }
        }
    }
}
