namespace NESCore.Mappers
{
    public interface IMapper
    {
        int ID { get; }
        byte PRGRomNumber { get; }
        int PRGRomSize { get; }
        byte CHRROMNumber { get; }
        int CHRROMSize { get; }

        RomBase Chr { get; }
        
        RomBase PrgRam { get; }

        bool TryRead(UInt16 address, out byte value);

        void Write(UInt16 address, byte value);
    }
}
