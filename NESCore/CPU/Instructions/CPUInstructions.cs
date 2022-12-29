namespace NESCore.CPU.Instructions
{
    //TODO: extract cpu addressing modes to their own methods

    public partial class CPUInstructions
    {
        //Instructions for the CPU. All instructions return with the elapsed cycles.
        //NOTE: inside the methods the PC points to the first parameter
        public Func<IBUS, IRegisters, byte>[] InstructionSet { get; private set; }

        public CPUInstructions()
        {
            InstructionSet = new Func<IBUS, IRegisters, byte>[255];

            InstructionSet[Opcodes.JMP_ABS] = JMP_ABS;
            InstructionSet[Opcodes.LDX_IMM] = LDX_IMM;
            InstructionSet[Opcodes.LDY_IMM] = LDY_IMM;
            InstructionSet[Opcodes.STX_ZERO] = STX_ZERO;
            InstructionSet[Opcodes.JSR_ABS] = JSR_ABS;
            InstructionSet[Opcodes.NOP] = NOP;
            InstructionSet[Opcodes.SEC] = SEC;
            InstructionSet[Opcodes.BCS] = BCS;
            InstructionSet[Opcodes.CLC] = CLC;
            InstructionSet[Opcodes.CLD] = CLD;
            InstructionSet[Opcodes.CLV] = CLV;
            InstructionSet[Opcodes.BCC] = BCC;
            InstructionSet[Opcodes.LDA_IMM] = LDA_IMM;
            InstructionSet[Opcodes.BEQ] = BEQ;
            InstructionSet[Opcodes.BNE] = BNE;
            InstructionSet[Opcodes.STA_ZERO] = STA_ZERO;
            InstructionSet[Opcodes.BIT_ZERO] = BIT_ZERO;
            InstructionSet[Opcodes.BVS] = BVS;
            InstructionSet[Opcodes.BVC] = BVC;
            InstructionSet[Opcodes.BPL] = BPL;
            InstructionSet[Opcodes.RTS] = RTS;
            InstructionSet[Opcodes.SEI] = SEI;
            InstructionSet[Opcodes.SED] = SED;
            InstructionSet[Opcodes.PHP] = PHP;
            InstructionSet[Opcodes.PLA] = PLA;
            InstructionSet[Opcodes.PLP] = PLP;
            InstructionSet[Opcodes.AND_IMM] = AND_IMM;
            InstructionSet[Opcodes.CMP_IMM] = CMP_IMM;
            InstructionSet[Opcodes.CPX_IMM] = CPX_IMM;
            InstructionSet[Opcodes.CPY_IMM] = CPY_IMM;
            InstructionSet[Opcodes.PHA] = PHA;
            InstructionSet[Opcodes.BMI] = BMI;
            InstructionSet[Opcodes.ORA_IMM] = ORA_IMM;
            InstructionSet[Opcodes.EOR_IMM] = EOR_IMM;
            InstructionSet[Opcodes.ADC_IMM] = ADC_IMM;
            InstructionSet[Opcodes.SBC_IMM] = SBC_IMM;
            InstructionSet[Opcodes.INY] = INY;
            InstructionSet[Opcodes.INX] = INX;
            InstructionSet[Opcodes.DEY] = DEY;
            InstructionSet[Opcodes.DEX] = DEX;
            InstructionSet[Opcodes.TAY] = TAY;
            InstructionSet[Opcodes.TAX] = TAX;
            InstructionSet[Opcodes.TYA] = TYA;
            InstructionSet[Opcodes.TXA] = TXA;
            InstructionSet[Opcodes.TSX] = TSX;
            InstructionSet[Opcodes.STX_ABS] = STX_ABS;
            InstructionSet[Opcodes.TXS] = TXS;
            InstructionSet[Opcodes.LDX_ABS] = LDX_ABS;
            InstructionSet[Opcodes.LDA_ABS] = LDA_ABS;
            InstructionSet[Opcodes.RTI] = RTI;
            InstructionSet[Opcodes.LSR_A] = LSR_A;
            InstructionSet[Opcodes.ASL_A] = ASL_A;
            InstructionSet[Opcodes.ROR_A] = ROR_A;
            InstructionSet[Opcodes.ROL_A] = ROL_A;
        }

        private static byte NOP(IBUS bus, IRegisters registers)
        {
            return 2;
        }

        private static ushort Fetch16(IBUS bus, IRegisters registers)
        {
            var low = Fetch(bus, registers);
            var high = Fetch(bus, registers);
            return (ushort)(high << 8 | low);
        }

        private static byte Fetch(IBUS bus, IRegisters registers)
        {
            return bus.Read(registers.PC++);
        }

        private static void Push16(IBUS bus, IRegisters registers, ushort value)
        {
            var pcLow = (byte)(value & 0xff);
            var pcHigh = (byte)((value & 0xff00) >> 8);

            Push8(bus, registers, pcHigh);
            Push8(bus, registers, pcLow);
        }

        private static void Push8(IBUS bus, IRegisters registers, byte value)
        {
            bus.Write((ushort)(0x100 | registers.SP--), value);
        }

        private static ushort Pop16(IBUS bus, IRegisters registers)
        {
            var low = Pop8(bus, registers);
            var high = Pop8(bus, registers);

            var address = (ushort)(high << 8 | low);
            return address;
        }

        private static byte Pop8(IBUS bus, IRegisters registers)
        {
            var sp = registers.SP + 1;
            registers.SP++;
            return bus.Read((ushort)(0x100 | sp));
        }

        class AddressingResult
        {
            public byte Value { get; init; }
            public ushort Address { get; init; }
        }

        private static AddressingResult AddressingImmediate(IBUS bus, IRegisters registers)
        {
            return new AddressingResult
            {
                Value = Fetch(bus, registers),
            };
        }

        // Reads a value from an absolute address
        private static AddressingResult AddressingAbsoluteWithValue(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);
            var value = bus.Read(address);
            return new AddressingResult
            {
                Value = value,
                Address = address,
            };
        }

        // Fetches the address of a memory location, but doesn't fetch the value
        private static AddressingResult AddressingAbsoulteAddressOnly(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);
            return new AddressingResult
            {
                Address = address,
            };
        }

        // Fetches the address of a zero page memory location, but doesn't fetch the value
        private static AddressingResult AddressingZeroAddressOnly(IBUS bus, IRegisters registers)
        {
            var address = Fetch(bus, registers);
            return new AddressingResult
            {
                Address = address,
            };
        }

        private static AddressingResult AddressingZeroWithValue(IBUS bus, IRegisters registers)
        {
            var address = Fetch(bus, registers);
            var value = bus.Read(address);
            return new AddressingResult
            {
                Address = address,
                Value = value,
            };
        }
    }
}
