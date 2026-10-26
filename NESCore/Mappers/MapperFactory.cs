using NESCore.Cartridge;
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

            if (header.MapperID == 0)
            {
                List<ChrBase> chr;
                if (chrRoms.Count == 0)
                {
                    chr = [new ChrRam()];
                }
                else
                {
                    chr = chrRoms;
                }

                return new Mapper0(prgRoms, chr, logger);
            }

            throw new UnknownMapperException(header.MapperID);
        }
    }
}