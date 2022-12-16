using NESCore.Mappers;

//TODO: Support iNES2.0 format

namespace NESCore.Cartridge
{
    public class CartridgeParser
    {
        private readonly IMapperFactory _mapperFactory;

        public CartridgeParser(IMapperFactory mapperFactory)
        {
            _mapperFactory = mapperFactory;
        }

        public ICartridge LoadCartridge(string filepath)
        {
            var rawBytes = File.ReadAllBytes(filepath);
            var header = new CartridgeHeader(rawBytes.AsSpan()[0..16]);

            var romsStart = 16;
            var romsSize = header.NumberOf16kROMBanks * 16 * 1024;
            var vromsStart = romsStart + romsSize;
            var vromsSize = header.NumberOf8kVROMBanks * 8 * 1024;

            var prgRoms = CreateROMs(rawBytes.AsSpan(romsStart, romsSize));
            var chrRoms = CreateVROMs(rawBytes.AsSpan(vromsStart, vromsSize));
            var mapper = _mapperFactory.CreateMapper(header, prgRoms, chrRoms);

            return new Cartridge(header, mapper);
        }

        private static List<ROM> CreateROMs(ReadOnlySpan<byte> romBytes)
        {
            var roms = new List<ROM>();
            for (int i = 0; i < romBytes.Length; i += 16 * 1024)
            {
                roms.Add(CreateROM(romBytes.Slice(i, 16 * 1024)));
            }
            return roms;
        }

        private static List<VROM> CreateVROMs(ReadOnlySpan<byte> romBytes)
        {
            var roms = new List<VROM>();
            for (int i = 0; i < romBytes.Length; i += 8 * 1024)
            {
                roms.Add(CreateVROM(romBytes.Slice(i, 8 * 1024)));
            }
            return roms;
        }

        private static ROM CreateROM(ReadOnlySpan<byte> rawBytes)
        {
            return new ROM(rawBytes.ToArray());
        }

        private static VROM CreateVROM(ReadOnlySpan<byte> rawBytes)
        {
            return new VROM(rawBytes.ToArray());
        }
    }
}
