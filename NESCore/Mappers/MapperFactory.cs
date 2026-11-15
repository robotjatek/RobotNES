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
                var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                return new Mapper0(prgRoms, chr, logger);
            } 
            else if (header.MapperID == 2)
            {
                // I don't think unrom should support chr ram
                var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                return new UnRom(prgRoms, chr, logger);
            }
            else if (header.MapperID == 3)
            {
                var chr = chrRoms.Count == 0 ? [new ChrRam()] : chrRoms;
                return new CnRom(prgRoms, chr, logger);
            }

            throw new UnknownMapperException(header.MapperID);
        }
    }
}