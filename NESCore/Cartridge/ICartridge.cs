using NESCore.Mappers;

namespace NESCore.Cartridge
{
    public interface ICartridge
    {
        ICartridgeHeader Header { get; }
        IMapper Mapper { get; }
        bool TryRead(ushort address, out byte value);
        void Write(ushort address, byte value);
    }
}
