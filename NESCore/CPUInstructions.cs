namespace NESCore
{
    public class Opcodes
    {
        public const int JMP_ABS = 0x4C;
    }

    public class CPUInstructions
    {
        //Instructions for the CPU. All instructions return with the elapsed cycles.
        public Func<IBUS, IRegisters, byte>[] InstructionSet { get; private set; }

        public CPUInstructions()
        {
            InstructionSet = new Func<IBUS, IRegisters, byte>[255];

            InstructionSet[Opcodes.JMP_ABS] = JMP_ABS;
        }

        private readonly Func<IBUS, IRegisters, byte> JMP_ABS = (bus, registers) =>
        {
            var low = bus.Read((UInt16)(registers.PC + 1));
            var high = bus.Read((UInt16)(registers.PC + 2));
            UInt16 address = (UInt16)(high << 8 | low);
            registers.PC = address;

            return 3;
        };
    }
}
