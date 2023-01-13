namespace NESCore.CPU
{
    public interface ICPU
    {
        void Cycle();
        void HandleNMI();

        // Full Fetch-Decode-Execute cycle. (In the foreseeable future I want to implement a per cycle basis, but for the first couple of instructions its better to have this I guess.)
        uint RunInstruction();
    }
}