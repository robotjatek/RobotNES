using NESCore.Cartridge;
using NESCore.Mappers.MMC1;
using Serilog;

namespace NESCore.Mappers
{
    public interface IMapperFactory
    {
        IMapper CreateMapper(ICartridgeHeader header, List<Rom> prgRoms, List<ChrBase> chrRoms);
    }

    public class MapperFactory(ILogger logger) : IMapperFactory
    {
        public IMapper CreateMapper(ICartridgeHeader header, List<Rom> prgRoms, List<ChrBase> chrRoms)
        {
            logger.Information("Creating mapper {HeaderMapperId}", header.MapperID);

            switch (header.MapperID)
            {
                case 0:
                {
                    var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                    return new Mapper0(prgRoms, chr, logger, header);
                }
                case 1:
                {
                    var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                    return new Mmc1(prgRoms, chr, logger);
                }
                case 2:
                {
                    // I don't think unrom should support chr ram
                    var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                    return new UnRom(prgRoms, chr, logger, header);
                }
                case 3:
                {
                    var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                    return new CnRom(prgRoms, chr, logger, header);
                }
                default:
                    throw new UnknownMapperException(header.MapperID);
            }
        }
    }
}