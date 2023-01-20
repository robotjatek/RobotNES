namespace NESCore.APU
{
    public interface IAPU
    {
        void Write(UInt16 address, byte data);
        byte Read(UInt16 address);
    }
}
