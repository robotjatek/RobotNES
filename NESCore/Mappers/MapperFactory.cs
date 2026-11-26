using NESCore.Cartridge;
using NESCore.Mappers.MMC1;
using Serilog;

namespace NESCore.Mappers
{
    public interface IMapperFactory
    {
        IMapper CreateMapper(ICartridgeHeader header, byte[] prgRoms, byte[] chrRoms, Func<IBUS> getBus);
    }

    public class MapperFactory(ILogger logger) : IMapperFactory
    {
        public IMapper CreateMapper(ICartridgeHeader header, byte[] prgRoms, byte[] chrRoms, Func<IBUS> getBus)
        {
            logger.Information("Creating mapper {HeaderMapperId}", header.MapperID);

            switch (header.MapperID)
            {
                case 0:
                {
                    var chr = chrRoms.Length == 0 ? new SlicedRom(8, new byte[8*1024], false) : new SlicedRom(8, chrRoms, true);
                    var prg = new SlicedRom(16, prgRoms, true);
                    return new Mapper0(prg, chr, logger, header);
                }
                // case 1:
                // {
                //     var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                //     return new Mmc1(prgRoms, chr, getBus, logger);
                // }
                // case 2:
                // {
                //     // I don't think unrom should support chr ram
                //     var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                //     return new UnRom(prgRoms, chr, logger, header);
                // }
                // case 3:
                // {
                //     var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                //     return new CnRom(prgRoms, chr, logger, header);
                // }
                // case 7:
                // {
                //     // BattleToads: NES2.0 8k CHR-RAM size: 8192
                //     var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                //     return new AoRom(prgRoms, chr, logger);
                // }
                default:
                    throw new UnknownMapperException(header.MapperID);
            }
        }
    }
}