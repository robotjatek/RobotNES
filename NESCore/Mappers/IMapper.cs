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

        RomBase Chr { get; }
        
        RomBase PrgRam { get; }

        bool TryRead(ushort address, out byte value);

        void Write(ushort address, byte value);
        
        MirroringMode GetMirroring();
    }
}
