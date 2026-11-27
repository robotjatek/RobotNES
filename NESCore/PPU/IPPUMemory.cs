namespace NESCore.PPU
{
    public interface IPPUMemory
    {
        void Write(ushort address, byte data);
        byte Read(ushort address);
        void OamWrite(byte address, byte data);
        byte OamRead(byte address);
        OamEntry GetOamEntry(int index);
        OamEntry GetOamEntryAtAddress(ushort address, int index);
    }
}
