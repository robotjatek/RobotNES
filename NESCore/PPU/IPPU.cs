namespace NESCore.PPU
{
    public interface IPPU
    {
        byte Read(UInt16 address);

        void Write(UInt16 address, byte value);
    }
}
