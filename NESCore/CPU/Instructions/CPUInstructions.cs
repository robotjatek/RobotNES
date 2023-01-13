using System.Runtime.InteropServices;

namespace NESCore.CPU.Instructions
{
    //TODO: return elapsed cycles in all addressing mode functions

    public partial class CPUInstructions
    {
        //Instructions for the CPU. All instructions return with the elapsed cycles.
        //NOTE: inside the methods the PC points to the first parameter
        public IReadOnlyList<Func<IBUS, IRegisters, byte>> InstructionSet { get; private set; }

        public CPUInstructions()
        {
            var instructions = new Func<IBUS, IRegisters, byte>[256];
            InstructionSet = instructions;

            for (int i = 0; i < instructions.Length; i++)
            {
                //Sets up the instructionset to throw on an unknown instruction
                instructions[i] = (IBUS bus, IRegisters registers) =>
                {
                    throw new NotImplementedException($"0x{i:X}@0x{registers.PC - 1:X2}");
                };
            }

            instructions[Opcodes.BRK] = BRK;
            instructions[Opcodes.JMP_ABS] = JMP_ABS;
            instructions[Opcodes.LDX_IMM] = LDX_IMM;
            instructions[Opcodes.LDY_IMM] = LDY_IMM;
            instructions[Opcodes.STX_ZERO] = STX_ZERO;
            instructions[Opcodes.JSR_ABS] = JSR_ABS;
            instructions[Opcodes.NOP] = NOP;
            instructions[Opcodes.SEC] = SEC;
            instructions[Opcodes.BCS] = BCS;
            instructions[Opcodes.CLC] = CLC;
            instructions[Opcodes.CLD] = CLD;
            instructions[Opcodes.CLV] = CLV;
            instructions[Opcodes.BCC] = BCC;
            instructions[Opcodes.LDA_IMM] = LDA_IMM;
            instructions[Opcodes.BEQ] = BEQ;
            instructions[Opcodes.BNE] = BNE;
            instructions[Opcodes.STA_ZERO] = STA_ZERO;
            instructions[Opcodes.BIT_ZERO] = BIT_ZERO;
            instructions[Opcodes.BVS] = BVS;
            instructions[Opcodes.BVC] = BVC;
            instructions[Opcodes.BPL] = BPL;
            instructions[Opcodes.RTS] = RTS;
            instructions[Opcodes.SEI] = SEI;
            instructions[Opcodes.SED] = SED;
            instructions[Opcodes.PHP] = PHP;
            instructions[Opcodes.PLA] = PLA;
            instructions[Opcodes.PLP] = PLP;
            instructions[Opcodes.AND_IMM] = AND_IMM;
            instructions[Opcodes.CMP_IMM] = CMP_IMM;
            instructions[Opcodes.CPX_IMM] = CPX_IMM;
            instructions[Opcodes.CPY_IMM] = CPY_IMM;
            instructions[Opcodes.PHA] = PHA;
            instructions[Opcodes.BMI] = BMI;
            instructions[Opcodes.ORA_IMM] = ORA_IMM;
            instructions[Opcodes.EOR_IMM] = EOR_IMM;
            instructions[Opcodes.ADC_IMM] = ADC_IMM;
            instructions[Opcodes.SBC_IMM] = SBC_IMM;
            instructions[Opcodes.INY] = INY;
            instructions[Opcodes.INX] = INX;
            instructions[Opcodes.DEY] = DEY;
            instructions[Opcodes.DEX] = DEX;
            instructions[Opcodes.TAY] = TAY;
            instructions[Opcodes.TAX] = TAX;
            instructions[Opcodes.TYA] = TYA;
            instructions[Opcodes.TXA] = TXA;
            instructions[Opcodes.TSX] = TSX;
            instructions[Opcodes.STX_ABS] = STX_ABS;
            instructions[Opcodes.TXS] = TXS;
            instructions[Opcodes.LDX_ABS] = LDX_ABS;
            instructions[Opcodes.LDA_ABS] = LDA_ABS;
            instructions[Opcodes.RTI] = RTI;
            instructions[Opcodes.LSR_A] = LSR_A;
            instructions[Opcodes.ASL_A] = ASL_A;
            instructions[Opcodes.ROR_A] = ROR_A;
            instructions[Opcodes.ROL_A] = ROL_A;
            instructions[Opcodes.LDA_ZERO] = LDA_ZERO;
            instructions[Opcodes.STA_ABS] = STA_ABS;
            instructions[Opcodes.LDA_IND_X] = LDA_IND_X;
            instructions[Opcodes.STA_IND_X] = STA_IND_X;
            instructions[Opcodes.ORA_IND_X] = ORA_IND_X;
            instructions[Opcodes.AND_IND_X] = AND_IND_X;
            instructions[Opcodes.EOR_IND_X] = EOR_IND_X;
            instructions[Opcodes.ADC_IND_X] = ADC_IND_X;
            instructions[Opcodes.CMP_IND_X] = CMP_IND_X;
            instructions[Opcodes.SBC_IND_X] = SBC_IND_X;
            instructions[Opcodes.LDY_ZERO] = LDY_ZERO;
            instructions[Opcodes.STY_ZERO] = STY_ZERO;
            instructions[Opcodes.LDX_ZERO] = LDX_ZERO;
            instructions[Opcodes.ORA_ZERO] = ORA_ZERO;
            instructions[Opcodes.AND_ZERO] = AND_ZERO;
            instructions[Opcodes.EOR_ZERO] = EOR_ZERO;
            instructions[Opcodes.ADC_ZERO] = ADC_ZERO;
            instructions[Opcodes.CMP_ZERO] = CMP_ZERO;
            instructions[Opcodes.SBC_ZERO] = SBC_ZERO;
            instructions[Opcodes.CPX_ZERO] = CPX_ZERO;
            instructions[Opcodes.CPY_ZERO] = CPY_ZERO;
            instructions[Opcodes.LSR_ZERO] = LSR_ZERO;
            instructions[Opcodes.ASL_ZERO] = ASL_ZERO;
            instructions[Opcodes.ROR_ZERO] = ROR_ZERO;
            instructions[Opcodes.ROL_ZERO] = ROL_ZERO;
            instructions[Opcodes.INC_ZERO] = INC_ZERO;
            instructions[Opcodes.DEC_ZERO] = DEC_ZERO;
            instructions[Opcodes.LDY_ABS] = LDY_ABS;
            instructions[Opcodes.STY_ABS] = STY_ABS;
            instructions[Opcodes.BIT_ABS] = BIT_ABS;
            instructions[Opcodes.ORA_ABS] = ORA_ABS;
            instructions[Opcodes.AND_ABS] = AND_ABS;
            instructions[Opcodes.EOR_ABS] = EOR_ABS;
            instructions[Opcodes.ADC_ABS] = ADC_ABS;
            instructions[Opcodes.CMP_ABS] = CMP_ABS;
            instructions[Opcodes.SBC_ABS] = SBC_ABS;
            instructions[Opcodes.CPX_ABS] = CPX_ABS;
            instructions[Opcodes.CPY_ABS] = CPY_ABS;
            instructions[Opcodes.LSR_ABS] = LSR_ABS;
            instructions[Opcodes.ASL_ABS] = ASL_ABS;
            instructions[Opcodes.ROR_ABS] = ROR_ABS;
            instructions[Opcodes.ROL_ABS] = ROL_ABS;
            instructions[Opcodes.INC_ABS] = INC_ABS;
            instructions[Opcodes.DEC_ABS] = DEC_ABS;
            instructions[Opcodes.LDA_IND_Y] = LDA_IND_Y;
            instructions[Opcodes.ORA_IND_Y] = ORA_IND_Y;
            instructions[Opcodes.AND_IND_Y] = AND_IND_Y;
            instructions[Opcodes.EOR_IND_Y] = EOR_IND_Y;
            instructions[Opcodes.ADC_IND_Y] = ADC_IND_Y;
            instructions[Opcodes.CMP_IND_Y] = CMP_IND_Y;
            instructions[Opcodes.SBC_IND_Y] = SBC_IND_Y;
            instructions[Opcodes.STA_IND_Y] = STA_IND_Y;
            instructions[Opcodes.JMP_INDIRECT] = JMP_INDIRECT;
            instructions[Opcodes.LDA_ABS_Y] = LDA_ABS_Y;
            instructions[Opcodes.ORA_ABS_Y] = ORA_ABS_Y;
            instructions[Opcodes.AND_ABS_Y] = AND_ABS_Y;
            instructions[Opcodes.EOR_ABS_Y] = EOR_ABS_Y;
            instructions[Opcodes.ADC_ABS_Y] = ADC_ABS_Y;
            instructions[Opcodes.CMP_ABS_Y] = CMP_ABS_Y;
            instructions[Opcodes.SBC_ABS_Y] = SBC_ABS_Y;
            instructions[Opcodes.STA_ABS_Y] = STA_ABS_Y;
            instructions[Opcodes.LDY_ZERO_X] = LDY_ZERO_X;
            instructions[Opcodes.STY_ZERO_X] = STY_ZERO_X;
            instructions[Opcodes.ORA_ZERO_X] = ORA_ZERO_X;
            instructions[Opcodes.AND_ZERO_X] = AND_ZERO_X;
            instructions[Opcodes.EOR_ZERO_X] = EOR_ZERO_X;
            instructions[Opcodes.ADC_ZERO_X] = ADC_ZERO_X;
            instructions[Opcodes.CMP_ZERO_X] = CMP_ZERO_X;
            instructions[Opcodes.SBC_ZERO_X] = SBC_ZERO_X;
            instructions[Opcodes.LDA_ZERO_X] = LDA_ZERO_X;
            instructions[Opcodes.STA_ZERO_X] = STA_ZERO_X;
            instructions[Opcodes.LSR_ZERO_X] = LSR_ZERO_X;
            instructions[Opcodes.ASL_ZERO_X] = ASL_ZERO_X;
            instructions[Opcodes.ROR_ZERO_X] = ROR_ZERO_X;
            instructions[Opcodes.ROL_ZERO_X] = ROL_ZERO_X;
            instructions[Opcodes.INC_ZERO_X] = INC_ZERO_X;
            instructions[Opcodes.DEC_ZERO_X] = DEC_ZERO_X;
            instructions[Opcodes.LDX_ZERO_Y] = LDX_ZERO_Y;
            instructions[Opcodes.STX_ZERO_Y] = STX_ZERO_Y;
            instructions[Opcodes.LDY_ABS_X] = LDY_ABS_X;
            instructions[Opcodes.ORA_ABS_X] = ORA_ABS_X;
            instructions[Opcodes.AND_ABS_X] = AND_ABS_X;
            instructions[Opcodes.EOR_ABS_X] = EOR_ABS_X;
            instructions[Opcodes.ADC_ABS_X] = ADC_ABS_X;
            instructions[Opcodes.CMP_ABS_X] = CMP_ABS_X;
            instructions[Opcodes.SBC_ABS_X] = SBC_ABS_X;
            instructions[Opcodes.LDA_ABS_X] = LDA_ABS_X;
            instructions[Opcodes.STA_ABS_X] = STA_ABS_X;
            instructions[Opcodes.LSR_ABS_X] = LSR_ABS_X;
            instructions[Opcodes.ASL_ABS_X] = ASL_ABS_X;
            instructions[Opcodes.ROR_ABS_X] = ROR_ABS_X;
            instructions[Opcodes.ROL_ABS_X] = ROL_ABS_X;
            instructions[Opcodes.INC_ABS_X] = INC_ABS_X;
            instructions[Opcodes.DEC_ABS_X] = DEC_ABS_X;
            instructions[Opcodes.LDX_ABS_Y] = LDX_ABS_Y;
            instructions[Opcodes.CLI] = CLI;
            
            instructions[Opcodes.NOP_ZERO_04] = NOP_ZERO;
            instructions[Opcodes.NOP_ZERO_44] = NOP_ZERO;
            instructions[Opcodes.NOP_ZERO_64] = NOP_ZERO;
            instructions[Opcodes.NOP_ABS_0C] = NOP_ABS;
            instructions[Opcodes.NOP_ZERO_X_14] = NOP_ZERO_X;
            instructions[Opcodes.NOP_ZERO_X_34] = NOP_ZERO_X;
            instructions[Opcodes.NOP_ZERO_X_54] = NOP_ZERO_X;
            instructions[Opcodes.NOP_ZERO_X_74] = NOP_ZERO_X;
            instructions[Opcodes.NOP_ZERO_X_D4] = NOP_ZERO_X;
            instructions[Opcodes.NOP_ZERO_X_F4] = NOP_ZERO_X;
            instructions[Opcodes.NOP_1A] = NOP;
            instructions[Opcodes.NOP_3A] = NOP;
            instructions[Opcodes.NOP_5A] = NOP;
            instructions[Opcodes.NOP_7A] = NOP;
            instructions[Opcodes.NOP_DA] = NOP;
            instructions[Opcodes.NOP_FA] = NOP;
            instructions[Opcodes.NOP_IMM_80] = NOP_IMM;
            instructions[Opcodes.NOP_ABS_X_1C] = NOP_ABS_X;
            instructions[Opcodes.NOP_ABS_X_3C] = NOP_ABS_X;
            instructions[Opcodes.NOP_ABS_X_5C] = NOP_ABS_X;
            instructions[Opcodes.NOP_ABS_X_7C] = NOP_ABS_X;
            instructions[Opcodes.NOP_ABS_X_DC] = NOP_ABS_X;
            instructions[Opcodes.NOP_ABS_X_FC] = NOP_ABS_X;
            
            instructions[Opcodes.LAX_IND_X] = LAX_IND_X;
            instructions[Opcodes.LAX_ZERO] = LAX_ZERO;
            instructions[Opcodes.LAX_ABS] = LAX_ABS;
            instructions[Opcodes.LAX_IND_Y] = LAX_IND_Y;
            instructions[Opcodes.LAX_ZERO_Y] = LAX_ZERO_Y;
            instructions[Opcodes.LAX_ABS_Y] = LAX_ABS_Y;
            
            instructions[Opcodes.SAX_IND_X] = SAX_IND_X;
            instructions[Opcodes.SAX_ZERO] = SAX_ZERO;
            instructions[Opcodes.SAX_ABS] = SAX_ABS;
            instructions[Opcodes.SAX_ZERO_Y] = SAX_ZERO_Y;
            
            instructions[Opcodes.SBC_IMM_EB] = SBC_IMM;

            instructions[Opcodes.DCP_IND_X] = DCP_IND_X;
            instructions[Opcodes.DCP_ZERO] = DCP_ZERO;
            instructions[Opcodes.DCP_ABS] = DCP_ABS;
            instructions[Opcodes.DCP_IND_Y] = DCP_IND_Y;
            instructions[Opcodes.DCP_ZERO_X] = DCP_ZERO_X;
            instructions[Opcodes.DCP_ABS_Y] = DCP_ABS_Y;
            instructions[Opcodes.DCP_ABS_X] = DCP_ABS_X;
            
            instructions[Opcodes.ISB_IND_X] = ISB_IND_X;
            instructions[Opcodes.ISB_ZERO] = ISB_ZERO;
            instructions[Opcodes.ISB_ABS] = ISB_ABS;
            instructions[Opcodes.ISB_IND_Y] = ISB_IND_Y;
            instructions[Opcodes.ISB_ZERO_X] = ISB_ZERO_X;
            instructions[Opcodes.ISB_ABS_Y] = ISB_ABS_Y;
            instructions[Opcodes.ISB_ABS_X] = ISB_ABS_X;
            
            instructions[Opcodes.SLO_IND_X] = SLO_IND_X;
            instructions[Opcodes.SLO_ZERO] = SLO_ZERO;
            instructions[Opcodes.SLO_ABS] = SLO_ABS;
            instructions[Opcodes.SLO_IND_Y] = SLO_IND_Y;
            instructions[Opcodes.SLO_ZERO_X] = SLO_ZERO_X;
            instructions[Opcodes.SLO_ABS_Y] = SLO_ABS_Y;
            instructions[Opcodes.SLO_ABS_X] = SLO_ABS_X;
            
            instructions[Opcodes.RLA_IND_X] = RLA_IND_X;
            instructions[Opcodes.RLA_ZERO] = RLA_ZERO;
            instructions[Opcodes.RLA_ABS] = RLA_ABS;
            instructions[Opcodes.RLA_IND_Y] = RLA_IND_Y;
            instructions[Opcodes.RLA_ZERO_X] = RLA_ZERO_X;
            instructions[Opcodes.RLA_ABS_Y] = RLA_ABS_Y;
            instructions[Opcodes.RLA_ABS_X] = RLA_ABS_X;
            
            instructions[Opcodes.SRE_IND_X] = SRE_IND_X;
            instructions[Opcodes.SRE_ZERO] = SRE_ZERO;
            instructions[Opcodes.SRE_ABS] = SRE_ABS;
            instructions[Opcodes.SRE_IND_Y] = SRE_IND_Y;
            instructions[Opcodes.SRE_ZERO_X] = SRE_ZERO_X;
            instructions[Opcodes.SRE_ABS_Y] = SRE_ABS_Y;
            instructions[Opcodes.SRE_ABS_X] = SRE_ABS_X;
            
            instructions[Opcodes.RRA_IND_X] = RRA_IND_X;
            instructions[Opcodes.RRA_ZERO] = RRA_ZERO;
            instructions[Opcodes.RRA_ABS] = RRA_ABS;
            instructions[Opcodes.RRA_IND_Y] = RRA_IND_Y;
            instructions[Opcodes.RRA_ZERO_X] = RRA_ZERO_X;
            instructions[Opcodes.RRA_ABS_Y] = RRA_ABS_Y;
            instructions[Opcodes.RRA_ABS_X] = RRA_ABS_X;
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
            public byte Value { get; set; }
            public ushort Address { get; init; }
            public byte Cycles { get; init; }

            public AddressingResult WithValueFromMemory(IBUS bus)
            {
                Value = bus.Read(Address);
                return this;
            }
        }

        private static AddressingResult AddressingImmediate(IBUS bus, IRegisters registers)
        {
            return new AddressingResult
            {
                Value = Fetch(bus, registers),
                Cycles = 2,
            };
        }

        // Reads a value from an absolute address
        private static AddressingResult AddressingAbsolute(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }

        private static AddressingResult AddressingAbsoluteAddressOnly(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);
            return new AddressingResult
            {
                Address = address,
                Cycles = 4,
            };
        }

        private static AddressingResult AddressingAbsoluteX(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteXAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }

        private static AddressingResult AddressingAbsoluteXAddressOnly(IBUS bus, IRegisters registers)
        {
            byte cycles = 4;
            var address = Fetch16(bus, registers);
            var page = address & 0xff00;
            address += registers.X;
            if ((address & 0xff00) != page)
            {
                cycles++;
            }

            return new AddressingResult
            {
                Address = address,
                Cycles = cycles
            };
        }

        private static AddressingResult AddressingAbsoluteY(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteYAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }

        private static AddressingResult AddressingAbsoluteYAddressOnly(IBUS bus, IRegisters registers)
        {
            byte cycles = 4;
            var address = Fetch16(bus, registers);
            var page = address & 0xff00;
            address += registers.Y;
            if ((address & 0xff00) != page)
            {
                cycles++;
            }

            return new AddressingResult
            {
                Address = address,
                Cycles = cycles
            };
        }

        private static AddressingResult AddressingZero(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }

        private static AddressingResult AddressingZeroAddressOnly(IBUS bus, IRegisters registers)
        {
            var address = Fetch(bus, registers);
            return new AddressingResult
            {
                Address = address,
                Cycles = 3,
            };
        }

        private static AddressingResult AddressingZeroX(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroXAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }

        private static AddressingResult AddressingZeroXAddressOnly(IBUS bus, IRegisters registers)
        {
            var address = (byte)((Fetch(bus, registers) + registers.X) & 0xFF);
            return new AddressingResult
            {
                Address = address,
                Cycles = 4,
            };
        }

        private static AddressingResult AddressingZeroY(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroYAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }

        private static AddressingResult AddressingZeroYAddressOnly(IBUS bus, IRegisters registers)
        {
            var address = (byte)((Fetch(bus, registers) + registers.Y) & 0xFF);
            return new AddressingResult
            {
                Address = address,
                Cycles = 4,
            };
        }

        private static AddressingResult AddressingIndirectX(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectXAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }

        private static AddressingResult AddressingIndirectXAddressOnly(IBUS bus, IRegisters registers)
        {
            var zeropageAddress = Fetch(bus, registers) + registers.X;
            var lowAddress = bus.Read((UInt16)(zeropageAddress & 0xff));
            var highAddress = bus.Read((UInt16)(zeropageAddress + 1 & 0xff));
            var address = (UInt16)(highAddress << 8 | lowAddress);

            return new AddressingResult
            {
                Address = address,
                Cycles = 6,
            };
        }

        private static AddressingResult AddressingIndirectYAddressOnly(IBUS bus, IRegisters registers)
        {
            byte cycles = 5;
            var zeroPageAddress = Fetch(bus, registers);
            var lowAddress = bus.Read((UInt16)(zeroPageAddress & 0xff));
            var highAddress = bus.Read((UInt16)((zeroPageAddress + 1) & 0xff));
            var address = (UInt16)((highAddress << 8 | lowAddress) & 0xffff);
            var page = address & 0xff00;
            address += registers.Y;
            if ((address & 0xff00) != page)
            {
                cycles++;
            }

            return new AddressingResult
            {
                Address = address,
                Cycles = cycles
            };
        }

        private static AddressingResult AddressingIndirectY(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectYAddressOnly(bus, registers).WithValueFromMemory(bus);
            return addressingResult;
        }
    }
}
