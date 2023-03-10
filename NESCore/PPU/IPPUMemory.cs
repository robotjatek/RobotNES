namespace NESCore.PPU
{
    public interface IPPUMemory
    {
        void Write(UInt16 address, byte data);
        byte Read(UInt16 address);
        void OamWrite(byte address, byte data);
        byte OamRead(byte address);
    }
}
