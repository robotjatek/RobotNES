using NESCore.Cartridge;

using Serilog;

namespace NESCore.Mappers
{
    public interface IMapperFactory
    {
        IMapper CreateMapper(ICartridgeHeader header, List<ROM> prgRoms, List<VROM> chrRoms);
    }

    public class MapperFactory : IMapperFactory
    {
        private readonly ILogger _logger;

        public MapperFactory(ILogger logger) { _logger = logger; }

        public IMapper CreateMapper(ICartridgeHeader header, List<ROM> prgRoms, List<VROM> chrRoms)
        {
            _logger.Information($"Creating mapper {header.MapperID}");

            if(header.MapperID == 0)
            {
                return new Mapper0(prgRoms, chrRoms, _logger);
            }

            throw new UnknownMapperException(header.MapperID);
        }
    }
}
