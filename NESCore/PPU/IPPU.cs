namespace NESCore.PPU
{
    public delegate void NMIEventHandler(byte[] framebuffer);

    public interface IPPU
    {
        byte Read(UInt16 address);

        void Write(UInt16 address, byte value);

        void Run(int cycles);

        event NMIEventHandler NMIEvent;
    }
}
