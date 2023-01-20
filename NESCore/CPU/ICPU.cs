namespace NESCore.CPU
{
    public interface ICPU
    {
        void HandleNMI();

        void HandleDMA(byte cpuPage);

        uint RunInstruction();
    }
}