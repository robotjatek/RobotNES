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
        private readonly byte[] _oam = new byte[256];

        public PPUMemory(ICartridge cartridge, ILogger logger)
        {
            _cartridge = cartridge;
            _logger = logger;
        }

        public byte Read(ushort address)
        {
            address &= 0x3fff;

            if (IsInPatternTableZero(address))
            {
                //TODO: maybe i need to split 8k CHRroms to two independent 4k ROMS
                return _cartridge.Mapper.Chr.Read(address);
            }
            else if (IsInPatternTableOne(address))
            {
                return _cartridge.Mapper.Chr.Read(address);
            }
            else if (IsInNameTable(address))
            {
                return _memory[UseNametableMirroring(address)];
            }
            else if (IsInPaletteMemory(address))
            {
                return _paletteMemory[GetPaletteIndex(address)];
            }

            throw new NotImplementedException();
        }

        public void Write(ushort address, byte data)
        {
            address &= 0x3fff;

            if (IsInPatternTableZero(address))
            {
                _cartridge.Mapper.Chr.Write(address, data);
            }
            else if (IsInPatternTableOne(address))
            {
                _cartridge.Mapper.Chr.Write(address, data);
            }
            else if (IsInNameTable(address))
            {
                _memory[UseNametableMirroring(address)] = data;
            }
            else if (IsInPaletteMemory(address))
            {
                _paletteMemory[GetPaletteIndex(address)] = (byte)(data & 0x3f);
            }
            else
            {
                _logger.Error($"Invalid write to ppu memory address: 0x{address:X4}");
            }
        }

        private int GetPaletteIndex(ushort address)
        {
            // ID 3f10, 3f14, 3f18, 3f1c mirror 3f00, 3f04, 3f08, 3f0c
            var index = address & 0x1F;
            return index switch
            {
                0x10 => 0x00,
                0x14 => 0x04,
                0x18 => 0x08,
                0x1C => 0x0C,
                _ => index
            };
        }

        public void OamWrite(byte address, byte data)
        {
            _oam[address] = data;
        }

        public byte OamRead(byte address)
        {
            return _oam[address];
        }

        public OamEntry GetOamEntry(int index)
        {
            return new OamEntry(_oam[index * 4], _oam[index * 4 + 1], _oam[index * 4 + 2], _oam[index * 4 + 3], index);
        }

        private UInt16 UseNametableMirroring(ushort address)
        {
            if (address >= 0x3000)
            {
                address &= 0x2FFF;
            }

            if (_cartridge.Header.FLAGS.FourScreenMirroring == FOUR_SCREEN_MIRRORING.ENABLED)
            {
                _logger.Error("4-screen mirroring is not supported");
                throw new NotImplementedException();
            }

            if (_cartridge.Header.FLAGS.Mirroring == MIRRORING.VERTICAL)
            {
                return (UInt16)(address & 0x7ff);
            }
            else if (_cartridge.Header.FLAGS.Mirroring == MIRRORING.HORIZONTAL)
            {
                return (UInt16)(((address & 0x800) >> 1) | (address & 0x3ff));
            }

            throw new NotImplementedException("Unsupported address");
        }

        private static bool IsInPatternTableZero(ushort address)
        {
            return address <= 0x0fff;
        }

        private static bool IsInPatternTableOne(ushort address)
        {
            return address is >= 0x1000 and <= 0x1fff;
        }

        private static bool IsInPaletteMemory(ushort address)
        {
            return address is >= 0x3f00 and <= 0x3fff;
        }

        private static bool IsInNameTable(ushort address)
        {
            return address is >= 0x2000 and <= 0x3EFF;
        }
    }
}