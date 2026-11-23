using NESCore.Mappers;

namespace NESCore.Cartridge
{
    public interface ICartridge
    {
        ICartridgeHeader Header { get; }
        IMapper Mapper { get; }
        bool TryRead(ushort address, out byte value);
        void Write(ushort address, byte value);
        // TODO: move mirror decoding here as it is mapper dependent!
        //  PPuMemory should call the new method instead of using the Header
        //  Mapper0 then should return the header defined mapping data (+cnrom +unrrom)
        //  Define a GetMirroring in IMapper
    }
}
