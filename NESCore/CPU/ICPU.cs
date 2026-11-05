namespace NESCore.CPU
{
    public interface ICpu
    {
        void HandleNmi();

        void HandleDma(byte cpuPage);

        uint RunInstruction();
    }
}