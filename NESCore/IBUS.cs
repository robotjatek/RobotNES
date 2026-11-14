namespace NESCore
{
    public delegate void OamDmaEventHandler(byte cpuPage);

    public interface IBUS
    {
        void Write(ushort address, byte data);

        byte Read(ushort address);

        event OamDmaEventHandler OamDmaEvent;
        
        void PpuOamDma(byte value);
        uint Cycles { get; }
        bool Irq { get; }
    }
}