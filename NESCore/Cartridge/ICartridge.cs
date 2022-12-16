using NESCore.Mappers;

namespace NESCore.Cartridge
{
    public interface ICartridge
    {
        ICartridgeHeader Header { get; }
        IMapper Mapper { get; }
        byte Read(ushort address);
        void Write(ushort address, byte value);
    }
}
