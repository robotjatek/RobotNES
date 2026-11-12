namespace NESCore.APU
{
    public interface IApu
    {
        void Tick();
        void Write(UInt16 address, byte data);
        byte Read(UInt16 address);
    }
}
