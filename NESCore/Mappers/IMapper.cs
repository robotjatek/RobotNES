namespace NESCore.Mappers
{
    public interface IMapper
    {
        int ID { get; }
        byte PRGRomNumber { get; }
        int PRGRomSize { get; }
        byte CHRROMNumber { get; }
        int CHRROMSize { get; }

        VROM CHRROM { get; }

        byte Read(UInt16 address);

        void Write(UInt16 address, byte value);
    }
}
