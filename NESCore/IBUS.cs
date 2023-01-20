namespace NESCore
{
    public delegate void OAMDMAEventHandler(byte cpuPage);

    public interface IBUS
    {
        void Write(UInt16 address, byte data);

        byte Read(UInt16 address);

        event OAMDMAEventHandler OAMDMAEvent;
    }
}