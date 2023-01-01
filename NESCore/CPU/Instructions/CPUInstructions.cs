namespace NESCore.CPU.Instructions
{
    //TODO: extract cpu addressing modes to their own methods
    //TODO: return elapsed cycles in all addressing mode functions

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
            InstructionSet[Opcodes.LDA_ZERO] = LDA_ZERO;
            InstructionSet[Opcodes.STA_ABS] = STA_ABS;
            InstructionSet[Opcodes.LDA_IND_X] = LDA_IND_X;
            InstructionSet[Opcodes.STA_IND_X] = STA_IND_X;
            InstructionSet[Opcodes.ORA_IND_X] = ORA_IND_X;
            InstructionSet[Opcodes.AND_IND_X] = AND_IND_X;
            InstructionSet[Opcodes.EOR_IND_X] = EOR_IND_X;
            InstructionSet[Opcodes.ADC_IND_X] = ADC_IND_X;
            InstructionSet[Opcodes.CMP_IND_X] = CMP_IND_X;
            InstructionSet[Opcodes.SBC_IND_X] = SBC_IND_X;
            InstructionSet[Opcodes.LDY_ZERO] = LDY_ZERO;
            InstructionSet[Opcodes.STY_ZERO] = STY_ZERO;
            InstructionSet[Opcodes.LDX_ZERO] = LDX_ZERO;
            InstructionSet[Opcodes.ORA_ZERO] = ORA_ZERO;
            InstructionSet[Opcodes.AND_ZERO] = AND_ZERO;
            InstructionSet[Opcodes.EOR_ZERO] = EOR_ZERO;
            InstructionSet[Opcodes.ADC_ZERO] = ADC_ZERO;
            InstructionSet[Opcodes.CMP_ZERO] = CMP_ZERO;
            InstructionSet[Opcodes.SBC_ZERO] = SBC_ZERO;
            InstructionSet[Opcodes.CPX_ZERO] = CPX_ZERO;
            InstructionSet[Opcodes.CPY_ZERO] = CPY_ZERO;
            InstructionSet[Opcodes.LSR_ZERO] = LSR_ZERO;
            InstructionSet[Opcodes.ASL_ZERO] = ASL_ZERO;
            InstructionSet[Opcodes.ROR_ZERO] = ROR_ZERO;
            InstructionSet[Opcodes.ROL_ZERO] = ROL_ZERO;
            InstructionSet[Opcodes.INC_ZERO] = INC_ZERO;
            InstructionSet[Opcodes.DEC_ZERO] = DEC_ZERO;
            InstructionSet[Opcodes.LDY_ABS] = LDY_ABS;
            InstructionSet[Opcodes.STY_ABS] = STY_ABS;
            InstructionSet[Opcodes.BIT_ABS] = BIT_ABS;
            InstructionSet[Opcodes.ORA_ABS] = ORA_ABS;
            InstructionSet[Opcodes.AND_ABS] = AND_ABS;
            InstructionSet[Opcodes.EOR_ABS] = EOR_ABS;
            InstructionSet[Opcodes.ADC_ABS] = ADC_ABS;
            InstructionSet[Opcodes.CMP_ABS] = CMP_ABS;
            InstructionSet[Opcodes.SBC_ABS] = SBC_ABS;
            InstructionSet[Opcodes.CPX_ABS] = CPX_ABS;
            InstructionSet[Opcodes.CPY_ABS] = CPY_ABS;
            InstructionSet[Opcodes.LSR_ABS] = LSR_ABS;
            InstructionSet[Opcodes.ASL_ABS] = ASL_ABS;
            InstructionSet[Opcodes.ROR_ABS] = ROR_ABS;
            InstructionSet[Opcodes.ROL_ABS] = ROL_ABS;
            InstructionSet[Opcodes.INC_ABS] = INC_ABS;
            InstructionSet[Opcodes.DEC_ABS] = DEC_ABS;
            InstructionSet[Opcodes.LDA_IND_Y] = LDA_IND_Y;
            InstructionSet[Opcodes.ORA_IND_Y] = ORA_IND_Y;
            InstructionSet[Opcodes.AND_IND_Y] = AND_IND_Y;
            InstructionSet[Opcodes.EOR_IND_Y] = EOR_IND_Y;
            InstructionSet[Opcodes.ADC_IND_Y] = ADC_IND_Y;
            InstructionSet[Opcodes.CMP_IND_Y] = CMP_IND_Y;
            InstructionSet[Opcodes.SBC_IND_Y] = SBC_IND_Y;
            InstructionSet[Opcodes.STA_IND_Y] = STA_IND_Y;
            InstructionSet[Opcodes.JMP_INDIRECT] = JMP_INDIRECT;
            InstructionSet[Opcodes.LDA_ABS_Y] = LDA_ABS_Y;
            InstructionSet[Opcodes.ORA_ABS_Y] = ORA_ABS_Y;
            InstructionSet[Opcodes.AND_ABS_Y] = AND_ABS_Y;
            InstructionSet[Opcodes.EOR_ABS_Y] = EOR_ABS_Y;
            InstructionSet[Opcodes.ADC_ABS_Y] = ADC_ABS_Y;
            InstructionSet[Opcodes.CMP_ABS_Y] = CMP_ABS_Y;
            InstructionSet[Opcodes.SBC_ABS_Y] = SBC_ABS_Y;
            InstructionSet[Opcodes.STA_ABS_Y] = STA_ABS_Y;
            InstructionSet[Opcodes.LDY_ZERO_X] = LDY_ZERO_X;
            InstructionSet[Opcodes.STY_ZERO_X] = STY_ZERO_X;
            InstructionSet[Opcodes.ORA_ZERO_X] = ORA_ZERO_X;
            InstructionSet[Opcodes.AND_ZERO_X] = AND_ZERO_X;
            InstructionSet[Opcodes.EOR_ZERO_X] = EOR_ZERO_X;
            InstructionSet[Opcodes.ADC_ZERO_X] = ADC_ZERO_X;
            InstructionSet[Opcodes.CMP_ZERO_X] = CMP_ZERO_X;
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
            public byte Cycles { get; init; }
        }

        private static AddressingResult AddressingImmediate(IBUS bus, IRegisters registers)
        {
            return new AddressingResult
            {
                Value = Fetch(bus, registers),
            };
        }

        // Reads a value from an absolute address
        private static AddressingResult AddressingAbsolute(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);
            var value = bus.Read(address);
            return new AddressingResult
            {
                Value = value,
                Address = address,
            };
        }

        private static AddressingResult AddressingAbsoluteX(IBUS bus, IRegisters registers)
        {
            byte cycles = 4;
            var address = Fetch16(bus, registers);
            var page = address & 0xff00;
            address += registers.X;
            if ((address & 0xff00) != page)
            {
                cycles++;
            }

            var value = bus.Read(address);
            return new AddressingResult
            {
                Value = value,
                Address = address,
                Cycles = cycles
            };
        }

        private static AddressingResult AddressingAbsoluteY(IBUS bus, IRegisters registers)
        {
            byte cycles = 4;
            var address = Fetch16(bus, registers);
            var page = address & 0xff00;
            address += registers.Y;
            if((address & 0xff00) != page)
            {
                cycles++;
            }

            var value = bus.Read(address);
            return new AddressingResult
            {
                Value = value,
                Address = address,
                Cycles = cycles
            };
        }

        private static AddressingResult AddressingZero(IBUS bus, IRegisters registers)
        {
            var address = Fetch(bus, registers);
            var value = bus.Read(address);
            return new AddressingResult
            {
                Address = address,
                Value = value,
            };
        }

        private static AddressingResult AddressingZeroX(IBUS bus, IRegisters registers)
        {
            var address = (byte)((Fetch(bus, registers) + registers.X) & 0xFF);
            var value = bus.Read(address);
            return new AddressingResult
            {
                Address = address,
                Value = value,
                Cycles = 4,
            };
        }

        private static AddressingResult AddressingIndirectX(IBUS bus, IRegisters registers)
        {
            var zeropageAddress = Fetch(bus, registers) + registers.X;
            var lowAddress = bus.Read((UInt16)(zeropageAddress & 0xff));
            var highAddress = bus.Read((UInt16)(zeropageAddress + 1 & 0xff));
            var address = (UInt16)(highAddress << 8 | lowAddress);
            var value = bus.Read(address);

            return new AddressingResult
            {
                Address = address,
                Value = value
            };
        }

        private static AddressingResult AddressingIndirectY(IBUS bus, IRegisters registers)
        {
            byte cycles = 5;
            var zeroPageAddress = Fetch(bus, registers);
            var lowAddress = bus.Read((UInt16)(zeroPageAddress&0xff));
            var highAddress = bus.Read((UInt16)((zeroPageAddress + 1) & 0xff));
            var address = (UInt16)((highAddress << 8 | lowAddress) & 0xffff);
            var page = address & 0xff00;
            address += registers.Y;
            if((address & 0xff00) != page)
            {
                cycles++;
            }
            var value = bus.Read(address);

            return new AddressingResult
            {
                Address = address,
                Value = value,
                Cycles = cycles
            };
        }
    }
}
