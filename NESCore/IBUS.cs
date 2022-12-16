namespace NESCore
{
    public interface IBUS
    {
        void Write(UInt16 address, byte data);

        byte Read(UInt16 address);
    }
}