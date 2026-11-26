namespace NESCore.Mappers
{
    public interface IMapper
    {
        enum MirroringMode
        {
            Horizontal,
            Vertical,
            OneScreenLower,
            OneScreenUpper,
            FourScreen
        }
        
        int Id { get; }

        SlicedRom Chr { get; }
        
        Ram PrgRam { get; }

        bool TryRead(ushort address, out byte value);

        void Write(ushort address, byte value);
        
        MirroringMode GetMirroring();
    }
}
