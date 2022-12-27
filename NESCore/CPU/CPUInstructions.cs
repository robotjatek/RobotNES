namespace NESCore.CPU
{
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
        }

        private static byte BCC(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetCarryFlag() == false;
            });
        }

        private static byte BEQ(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetZeroFlag() == true;
            });
        }

        private static byte BNE(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetZeroFlag() == false;
            });
        }

        private static byte BCS(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetCarryFlag() == true;
            });
        }

        private static byte BVC(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetOverflowFlag() == false;
            });
        }

        private static byte BVS(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetOverflowFlag() == true;
            });
        }

        private static byte BPL(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetNegativeFlag() == false;
            });
        }

        private static byte BMI(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetNegativeFlag() == true;
            });
        }

        private static byte CLC(IBUS bus, IRegisters registers)
        {
            registers.SetCarryFlag(false);
            return 2;
        }

        private static byte CLD(IBUS bus, IRegisters registers)
        {
            registers.SetDecimalFlag(false);
            return 2;
        }

        private static byte CLV(IBUS bus, IRegisters registers)
        {
            registers.SetOverflowFlag(false);
            return 2;
        }

        private static byte SEC(IBUS bus, IRegisters registers)
        {
            registers.SetCarryFlag(true);
            return 2;
        }

        private static byte SEI(IBUS bus, IRegisters registers)
        {
            registers.SetInterruptDisableFlag(true);
            return 2;
        }

        private static byte SED(IBUS bus, IRegisters registers)
        {
            registers.SetDecimalFlag(true);
            return 2;
        }

        private static byte JMP_ABS(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);
            registers.PC = address;

            return 3;
        }

        private static void LDA(byte value, IRegisters registers)
        {
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte LDA_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDA(addressingResult.Value, registers);
            return 2;
        }

        private static byte LDA_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteWithValue(bus, registers);
            LDA(addressingResult.Value, registers);
            return 4;
        }

        private static void LDX(byte value, IRegisters registers)
        {
            registers.X = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte LDX_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDX(addressingResult.Value, registers);
            return 2;
        }

        private static byte LDX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteWithValue(bus, registers);
            LDX(addressingResult.Value, registers);
            return 4;
        }

        private static void LDY(byte value, IBUS bus, IRegisters registers)
        {
            registers.Y = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte LDY_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDY(addressingResult.Value, bus, registers);
            return 2;
        }

        private static void STA(ushort address, IBUS bus, IRegisters registers)
        {
            bus.Write(address, registers.A);
        }

        private static byte STA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroAddressOnly(bus, registers);
            STA(addressingResult.Address, bus, registers);
            return 3;
        }

        private static byte STX_ZERO(IBUS bus, IRegisters registers)
        {
            var address = AddressingZeroAddressOnly(bus, registers).Address;
            bus.Write(address, registers.X);
            return 3; //1(opcode fetch) + 1 (1 byte fetch from memory) + 1 (1 byte write to memory)
        }

        private static byte STX_ABS(IBUS bus, IRegisters registers)
        {
            var address = AddressingAbsoulteAddressOnly(bus, registers).Address;
            bus.Write(address, registers.X);
            return 4; //1(opcode fetch) + 2 (2 byte fetch from memory) + 1 (1 byte write to memory)
        }

        private static byte BIT_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZeroWithValue(bus, registers).Value;

            var isNegative = (value & 0x80) == 0x80;
            registers.SetNegativeFlag(isNegative);

            var overflow = (value & 0x40) == 0x40;
            registers.SetOverflowFlag(overflow);

            var isZero = (value & registers.A) == 0;
            registers.SetZeroFlag(isZero);

            return 3;
        }

        private static byte AND_IMM(IBUS bus, IRegisters registers)
        {
            var mask = Fetch(bus, registers);
            var value = (byte)(registers.A & mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
            return 2;
        }

        private static byte ORA_IMM(IBUS bus, IRegisters registers)
        {
            var mask = Fetch(bus, registers);
            var value = (byte)(registers.A | mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
            return 2;
        }

        private static byte EOR_IMM(IBUS bus, IRegisters registers)
        {
            var mask = Fetch(bus, registers);
            var value = (byte)(registers.A ^ mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
            return 2;
        }

        private static byte CMP_IMM(IBUS bus, IRegisters registers)
        {
            var imm = Fetch(bus, registers);
            var tmp = registers.A - imm;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.A >= imm);
            registers.SetNegativeFlag((tmp & 0x80) > 0);

            return 2;
        }

        private static byte CPX_IMM(IBUS bus, IRegisters registers)
        {
            var imm = Fetch(bus, registers);
            var tmp = registers.X - imm;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.X >= imm);
            registers.SetNegativeFlag((tmp & 0x80) > 0);

            return 2;
        }

        private static byte CPY_IMM(IBUS bus, IRegisters registers)
        {
            var imm = Fetch(bus, registers);
            var tmp = registers.Y - imm;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.Y >= imm);
            registers.SetNegativeFlag((tmp & 0x80) > 0);

            return 2;
        }

        private static byte JSR_ABS(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);

            Push16(bus, registers, (ushort)(registers.PC - 1));
            registers.PC = address;

            return 6;
        }

        private static byte RTS(IBUS bus, IRegisters registers)
        {
            var address = (ushort)(Pop16(bus, registers) + 1);
            registers.PC = address;

            return 6;
        }

        private static byte PHA(IBUS bus, IRegisters registers)
        {
            Push8(bus, registers, registers.A);
            return 3;
        }

        private static byte PHP(IBUS bus, IRegisters registers)
        {
            Push8(bus, registers, registers.STATUS);
            return 3;
        }

        private static byte PLA(IBUS bus, IRegisters registers)
        {
            var value = Pop8(bus, registers);
            registers.A = value;
            registers.SetNegativeFlag((sbyte)value < 0);
            registers.SetZeroFlag(value == 0);
            return 4;
        }

        private static byte PLP(IBUS bus, IRegisters registers)
        {
            var value = Pop8(bus, registers);
            registers.STATUS = value;
            return 4;
        }

        private static byte ADC_IMM(IBUS bus, IRegisters registers)
        {
            var operand = Fetch(bus, registers);
            var result = registers.A + operand;

            if (registers.GetCarryFlag() == true)
            {
                result++;
            }

            registers.SetCarryFlag(result > 255);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            //var overflow = ((registers.A ^ result) & (operand ^ result) & 0x80) > 0; //checks if the two operands and the result have differing sign bits.  If they do, the operation resulted in an overflow
            registers.SetOverflowFlag(result > 127 || result < -128);

            registers.A = (byte)result;
            return 2;
        }

        private static byte SBC_IMM(IBUS bus, IRegisters registers)
        {
            var operand = (sbyte)~Fetch(bus, registers);
            //var result = registers.A + operand;
            var result = (sbyte)registers.A + operand;
            if (registers.GetCarryFlag() == true)
            {
                result++;
            }

            registers.SetCarryFlag((sbyte)result >= 0);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            //var overflow = ((registers.A ^ result) & (operand ^ result) & 0x80) > 0; // overflow can be detected this way, but it looks like the solution below is sufficient when I downcast the result to sbyte in carry check and use range check in the overflow check on the original int result.
            //registers.SetOverflowFlag(overflow);
            registers.SetOverflowFlag(result > 127 || result < -128);

            registers.A = (byte)result;
            return 2;
        }

        private static byte INX(IBUS bus, IRegisters registers)
        {
            registers.X++;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte INY(IBUS bus, IRegisters registers)
        {
            registers.Y++;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }

        private static byte DEX(IBUS bus, IRegisters registers)
        {
            registers.X--;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte DEY(IBUS bus, IRegisters registers)
        {
            registers.Y--;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }

        private static byte TAX(IBUS bus, IRegisters registers)
        {
            registers.X = registers.A;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte TSX(IBUS bus, IRegisters registers)
        {
            registers.X = registers.SP;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte TAY(IBUS bus, IRegisters registers)
        {
            registers.Y = registers.A;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }

        private static byte TXA(IBUS bus, IRegisters registers)
        {
            registers.A = registers.X;
            registers.SetZeroFlag(registers.A == 0);
            registers.SetNegativeFlag((registers.A & 0x80) > 0);
            return 2;
        }

        private static byte TYA(IBUS bus, IRegisters registers)
        {
            registers.A = registers.Y;
            registers.SetZeroFlag(registers.A == 0);
            registers.SetNegativeFlag((registers.A & 0x80) > 0);
            return 2;
        }

        private static byte TXS(IBUS bus, IRegisters registers)
        {
            registers.SP = registers.X;
            return 2;
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

        private static byte BranchInstruction(IBUS bus, IRegisters registers, Func<IRegisters, bool> condition)
        {
            byte cycles = 2;
            sbyte offset = (sbyte)Fetch(bus, registers);
            if (condition(registers))
            {
                var oldAddress = registers.PC;
                registers.PC = (ushort)(registers.PC + offset);
                cycles++;
                if ((oldAddress & 0xff00) != (registers.PC & 0xff00))
                {
                    cycles++;
                }
            }

            return cycles;
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
