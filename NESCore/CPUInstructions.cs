namespace NESCore
{
    public class Opcodes
    {
        public const int JMP_ABS = 0x4C;
        public const int LDX_IMM = 0xA2;
        public const int STX_ZERO = 0x86;
    }

    //TODO: extract cpu addressing modes to their own methods

    public class CPUInstructions
    {
        //Instructions for the CPU. All instructions return with the elapsed cycles.
        public Func<IBUS, IRegisters, byte>[] InstructionSet { get; private set; }

        public CPUInstructions()
        {
            InstructionSet = new Func<IBUS, IRegisters, byte>[255];

            InstructionSet[Opcodes.JMP_ABS] = JMP_ABS;
            InstructionSet[Opcodes.LDX_IMM] = LDX_IMM;
            InstructionSet[Opcodes.STX_ZERO] = STX_ZERO;
        }

        private readonly Func<IBUS, IRegisters, byte> JMP_ABS = (bus, registers) =>
        {
            var low = Fetch(bus, registers);
            var high = Fetch(bus, registers);
            var address = (UInt16)(high << 8 | low);
            registers.PC = address;

            return 3;
        };

        private readonly Func<IBUS, IRegisters, byte> LDX_IMM = (bus, registers) =>
        {
            var immediateValue = Fetch(bus, registers);
            registers.X = immediateValue;
            registers.SetZeroFlag(immediateValue == 0);
            registers.SetNegativeFlag((sbyte)immediateValue < 0);
            return 2;
        };

        private readonly Func<IBUS, IRegisters, byte> STX_ZERO = (bus, registers) =>
        {
            var address = Fetch(bus, registers);
            bus.Write(address, registers.X);
            return 3; //1(opcode fetch) + 1 (1byte fetch from memory) + 1 (1byte write to memory)
        };

        private static byte Fetch(IBUS bus, IRegisters registers)
        {
            return bus.Read(registers.PC++);
        }
    }
}
