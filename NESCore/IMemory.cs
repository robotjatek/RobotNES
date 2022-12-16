namespace NESCore
{
    public interface IMemory
    {
        void Write(UInt16 address, byte value);

        byte Read(UInt16 address);
    }
}
