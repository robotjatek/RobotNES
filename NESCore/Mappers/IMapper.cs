namespace NESCore.Mappers
{
    public interface IMapper
    {
        int Id { get; }
        byte PrgRomNumber { get; }
        int PrgRomSize { get; }
        byte ChrRomNumber { get; }
        int ChrRomSize { get; }

        RomBase Chr { get; }
        
        RomBase PrgRam { get; }

        bool TryRead(ushort address, out byte value);

        void Write(ushort address, byte value);
    }
}
