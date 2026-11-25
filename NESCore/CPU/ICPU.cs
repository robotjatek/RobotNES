namespace NESCore.CPU
{
    public interface ICpu
    {
        void HandleDma(byte cpuPage);

        void RunInstruction();
    }
}