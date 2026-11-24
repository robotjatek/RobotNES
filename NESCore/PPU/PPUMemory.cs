using NESCore.Cartridge;
using NESCore.Mappers;
using Serilog;

namespace NESCore.PPU
{
    public class PpuMemory(ICartridge cartridge, ILogger logger) : IPPUMemory
    {
        private readonly byte[] _memory = new byte[0x800];
        private readonly byte[] _paletteMemory = new byte[32];
        private readonly byte[] _oam = new byte[256];

        public byte Read(ushort address)
        {
            address &= 0x3fff;

            if (IsInPatternTableZero(address))
            {
                return cartridge.Mapper.Chr.Read(address);
            }
            else if (IsInPatternTableOne(address))
            {
                return cartridge.Mapper.Chr.Read(address);
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
                cartridge.Mapper.Chr.Write(address, data);
            }
            else if (IsInPatternTableOne(address))
            {
                cartridge.Mapper.Chr.Write(address, data);
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
                logger.Error("Invalid write to ppu memory address: 0x{Address:X4}", address);
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
            return GetOamEntryAtAddress((byte)(index * 4), index);
        }

        public OamEntry GetOamEntryAtAddress(ushort address, int index)
        {
            return new OamEntry(
                _oam[address],
                _oam[(byte)(address + 1)],
                _oam[(byte)(address + 2)],
                _oam[(byte)(address + 3)],
                index);
        }

        private ushort UseNametableMirroring(ushort address)
        {
            if (address >= 0x3000)
            {
                address &= 0x2FFF;
            }
            
            switch (cartridge.Mapper.GetMirroring())
            {
                case IMapper.MirroringMode.FourScreen:
                    logger.Error("This mirroring mode is not supported: {Mirroring}", IMapper.MirroringMode.FourScreen);
                    throw new NotImplementedException();
                case IMapper.MirroringMode.Vertical:
                    return (ushort)(address & 0x7ff);
                case IMapper.MirroringMode.Horizontal:
                    return (ushort)(((address & 0x800) >> 1) | (address & 0x3ff));
                case IMapper.MirroringMode.OneScreenLower:
                    return (ushort)(address & 0x3FF);
                case IMapper.MirroringMode.OneScreenUpper:
                    return (ushort)((address & 0x3FF) | 0x400);
                default:
                    throw new NotImplementedException("Unsupported address");
            }
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