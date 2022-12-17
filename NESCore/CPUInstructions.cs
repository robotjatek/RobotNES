namespace NESCore
{
    public class Opcodes
    {
        public const int JMP_ABS = 0x4C;
        public const int LDX_IMM = 0xA2;
        public const int STX_ZERO = 0x86;
        public const int JSR_ABS = 0x20;
        public const int NOP = 0xEA;
        public const int SEC = 0x38;
        public const int BCS = 0xB0;
    }

    //TODO: extract cpu addressing modes to their own methods

    public class CPUInstructions
    {
        //Instructions for the CPU. All instructions return with the elapsed cycles.
        //NOTE: inside the methods the PC points to the first parameter
        public Func<IBUS, IRegisters, byte>[] InstructionSet { get; private set; }

        public CPUInstructions()
        {
            InstructionSet = new Func<IBUS, IRegisters, byte>[255];

            InstructionSet[Opcodes.JMP_ABS] = JMP_ABS;
            InstructionSet[Opcodes.LDX_IMM] = LDX_IMM;
            InstructionSet[Opcodes.STX_ZERO] = STX_ZERO;
            InstructionSet[Opcodes.JSR_ABS] = JSR_ABS;
            InstructionSet[Opcodes.NOP] = NOP;
            InstructionSet[Opcodes.SEC] = SEC;
            InstructionSet[Opcodes.BCS] = BCS;
        }

        private byte BCS(IBUS bus, IRegisters registers)
        {
            byte cycles = 2;
            sbyte offset = (sbyte)Fetch(bus, registers);
            if(registers.GetCarryFlag())
            {
                var oldAddress = registers.PC;
                registers.PC = (ushort)(registers.PC + offset);
                cycles++;
                if((oldAddress&0xff00) != (registers.PC&0xff00))
                {
                    cycles++;
                }
            }

            return cycles;
        }

        private byte SEC(IBUS bus, IRegisters registers)
        {
            registers.SetCarryFlag(true);
            return 2;
        }

        private static byte JMP_ABS(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);
            registers.PC = address;

            return 3;
        }

        private static byte LDX_IMM(IBUS bus, IRegisters registers)
        {
            var immediateValue = Fetch(bus, registers);
            registers.X = immediateValue;
            registers.SetZeroFlag(immediateValue == 0);
            registers.SetNegativeFlag((sbyte)immediateValue < 0);
            return 2;
        }

        private static byte STX_ZERO(IBUS bus, IRegisters registers)
        {
            var address = Fetch(bus, registers);
            bus.Write(address, registers.X);
            return 3; //1(opcode fetch) + 1 (1byte fetch from memory) + 1 (1byte write to memory)
        }

        private static byte JSR_ABS(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);

            Push16(bus, registers, (UInt16)(registers.PC - 1));
            registers.PC = address;

            return 6;
        }

        private static byte NOP(IBUS bus, IRegisters registers)
        {
            return 2;
        }

        private static UInt16 Fetch16(IBUS bus, IRegisters registers)
        {
            var low = Fetch(bus, registers);
            var high = Fetch(bus, registers);
            return (UInt16)(high << 8 | low);
        }

        private static byte Fetch(IBUS bus, IRegisters registers)
        {
            return bus.Read(registers.PC++);
        }

        private static void Push16(IBUS bus, IRegisters registers, UInt16 value)
        {
            var pcLow = (byte)(value & 0xff);
            var pcHigh = (byte)((value & 0xff00) >> 8);

            Push8(bus, registers, pcLow);
            Push8(bus, registers, pcHigh);
        }

        private static void Push8(IBUS bus, IRegisters registers, byte value)
        {
            bus.Write((UInt16)(0x100 | registers.SP--), value);
        }
    }
}
