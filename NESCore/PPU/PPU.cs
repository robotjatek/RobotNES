using Serilog;

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

        private UInt16 _address = 0;
        private bool _addressLatch = false;

        public event NMIEventHandler? NMIEvent;

        public PPU(IPPURegisters registers, IPPUMemory ppuMemory, ILogger logger)
        {
            _logger = logger;
            _registers = registers;
            _ppuMemory = ppuMemory;
        }

        public byte Read(ushort address)
        {
            //TODO: replace this branching with an indexed array (_ppuMemory[address & 0x7])
            if (address == 0x2000)
            {
                _logger.Warning("PPU control register read 0x2000. Normally this should not happen.");
                return _registers.Control;
            }
            else if (address == 0x2001)
            {
                _logger.Warning("PPU mask register read 0x2001. Normally this should not happen.");
                return _registers.Mask;
            }
            else if (address == 0x2002)
            {
                var data = (byte)(_registers.Status & 0xE0);  //TODO: openbus behaviour. Return previous data from the ppu databuffer on the lower bits
                _registers.SetVBlankFlag(false);
                //TODO: also clear address latch (used by PPU addr & ppu scroll)
                return data;
            }
            else if(address == 0x2005)
            {
                _logger.Warning("PPU scroll register read 0x2005. Normally this should not happen.");
                return _registers.Scroll;
            }
            else if (address == 0x2006)
            {
                _logger.Warning("PPU address register read 0x2006. Normally this should not happen.");
                return _registers.Address;
            }

            _logger.Error($"Unsupported PPU read from: 0x{address:X4}");
            throw new NotImplementedException();

        }

        public void Write(ushort address, byte value)
        {
            if (address == 0x2000)
            {
                _registers.Control = value;
            }
            else if(address == 0x2001)
            {
                _registers.Mask = value;
            }
            else if(address == 0x2005)
            {
                _registers.Scroll = value;
            }
            else if(address == 0x2006)
            {
                _registers.Address = value;
                if(_addressLatch == false)
                {
                    _address |= (UInt16)(value << 8);
                }
                else
                {
                    _address |= value;
                }
                _addressLatch = !_addressLatch;
            }
            else if(address == 0x2007)
            {
                _ppuMemory.Write(_address, value);
            }
            else
            {

                _logger.Error($"Unsupported PPU write to: 0x{address:X4}");
                throw new NotImplementedException();
            }
        }

        public void Run(int cycles)
        {
            for (int i = 0; i < cycles; i++)
            {
                Cycle();
            }
        }

        private void Cycle()
        {
            if (_scanlines >= 0 && _scanlines < 240)
            {
                //Cycle visible scanline
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
                        NMIEvent?.Invoke();
                    }
                }
            }
            else if(_scanlines == 261)
            {
                //cycle prerender scanline

                _registers.SetVBlankFlag(false);
                _registers.SetOverflowFlag(false);
                _registers.SetSpriteZeroHitFlag(false);
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
