using NESCore.Mappers;

//TODO: Support iNES2.0 format
//TODO: detect data after rom data
namespace NESCore.Cartridge
{
    public class CartridgeParser(IMapperFactory mapperFactory)
    {
        public ICartridge LoadCartridge(string filepath, Func<IBUS> getBus)
        {
            var rawBytes = File.ReadAllBytes(filepath);
            var header = new CartridgeHeader(rawBytes.AsSpan()[0..16]);

            const int romsStart = 16;
            var romsSize = header.NumberOf16kROMBanks * 16 * 1024;
            var vromsStart = romsStart + romsSize;
            var vromsSize = header.NumberOf8kVROMBanks * 8 * 1024;
            
            var mapper = mapperFactory.CreateMapper(header, [.. rawBytes.AsSpan(romsStart, romsSize)],
                [.. rawBytes.AsSpan(vromsStart, vromsSize)], getBus);

            return new Cartridge(header, mapper);
        }
    }
}
