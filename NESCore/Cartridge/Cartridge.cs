using NESCore.Mappers;
//TODO: PRG RAM

namespace NESCore.Cartridge
{
    public class Cartridge : ICartridge
    {
        public ICartridgeHeader Header { get; private set; }

        public IMapper Mapper { get; private set; }

        public Cartridge(ICartridgeHeader header, IMapper mapper)
        {
            Header = header;
            Mapper = mapper;
        }

        public byte Read(ushort address)
        {
            return Mapper.Read(address);
        }

        public void Write(ushort address, byte value)
        {
            Mapper.Write(address, value);
        }
    }
}