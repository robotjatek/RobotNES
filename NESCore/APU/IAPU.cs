namespace NESCore.APU
{
    public interface IApu
    {
        public delegate void SamplesReadyHandler(float[] samples, int count);
        event SamplesReadyHandler? SamplesReady;
        bool IrqFlag { get; }
        void Tick();
        void Write(ushort address, byte data);
        byte Read(ushort address);
    }
}
