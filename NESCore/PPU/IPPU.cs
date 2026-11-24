namespace NESCore.PPU
{
    public delegate void NMIEventHandler(byte[] framebuffer);

    // Raised at the start of every VBlank regardless of the NMI enable bit in PPU control ($2000 bit 7).
    // The PPU produces a rendered framebuffer whether or not the ROM asks for an NMI, so display
    // updates must not be gated on the NMI enable bit.
    public delegate void VBlankEventHandler(byte[] framebuffer);

    public interface IPPU
    {
        byte Read(UInt16 address);

        void Write(UInt16 address, byte value);

        void Run(uint cycles);

        void OamDMAWrite(byte value);

        event NMIEventHandler NMIEvent;

        event VBlankEventHandler? VBlank;
    }
}