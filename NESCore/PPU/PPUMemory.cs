using NESCore.Cartridge;

using Serilog;

namespace NESCore.PPU
{
    public class PPUMemory : IPPUMemory
    {
        private readonly ICartridge _cartridge;
        private readonly ILogger _logger;
        private readonly byte[] _memory = new byte[0x800];
        private readonly byte[] _paletteMemory = new byte[32];

        public PPUMemory(ICartridge cartridge, ILogger logger)
        {
            _cartridge = cartridge;
            _logger = logger;
        }

        public byte Read(ushort address)
        {
            if (IsInPatternTableZero(address))
            {
                //TODO: maybe i need to split 8k CHRroms to two independent 4k ROMS
                return _cartridge.Mapper.CHRROM.Read(address);
            }
            else if (IsInPatternTableOne(address))
            {
                return _cartridge.Mapper.CHRROM.Read(address);
            }
            else if (IsInNameTable(address))
            {
                return _memory[UseNametableMirroring(address)];
            }
            else if (IsInPaletteMemory(address))
            {
                return _paletteMemory[address & 0x1f];
            }

            throw new NotImplementedException();
        }

        public void Write(ushort address, byte data)
        {
            if (IsInPatternTableZero(address))
            {
                //TODO: maybe these writes are CHR-RAM writes?
                _logger.Error("Unsupported write to pattern table 0 through ppu");
                throw new NotImplementedException();
            }
            else if (IsInPatternTableOne(address))
            {
                //TODO: maybe these writes are CHR-RAM writes?
                _logger.Error("Unsupported write to pattern table 1 through ppu");
                throw new NotImplementedException();
            }
            else if (IsInNameTable(address))
            {
                _memory[UseNametableMirroring(address)] = data;
            }
            else if (IsInPaletteMemory(address))
            {
                _paletteMemory[address & 0x1f] = data;
            }
        }

        private UInt16 UseNametableMirroring(ushort address)
        {
            if(_cartridge.Header.FLAGS.FourScreenMirroring == FOUR_SCREEN_MIRRORING.ENABLED)
            {
                _logger.Error("4-screen mirroring is not supported");
                throw new NotImplementedException();
            }

            if (_cartridge.Header.FLAGS.Mirroring == MIRRORING.VERTICAL)
            {
                address = (UInt16)(address & (~0x800));
            }
            else if (_cartridge.Header.FLAGS.Mirroring == MIRRORING.HORIZONTAL)
            {
                address = (UInt16)(address & (~0x400));
            }

            return (UInt16)(address & 0x7ff);
        }

        private static bool IsInPatternTableZero(ushort address)
        {
            return address >= 0 && address <= 0x0fff;
        }

        private static bool IsInPatternTableOne(ushort address)
        {
            return address >= 0x1000 && address <= 0x1fff;
        }

        private static bool IsInPaletteMemory(ushort address)
        {
            return address >= 0x3f00 && address <= 0x3fff;
        }

        private static bool IsInNameTable(ushort address)
        {
            return address >= 0x2000 && address <= 0x3EFF;
        }
    }
}
