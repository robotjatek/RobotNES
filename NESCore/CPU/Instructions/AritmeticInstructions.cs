namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static void ADC(byte operand, IRegisters registers)
        {
            var result = registers.A + operand;

            if (registers.GetCarryFlag() == true)
            {
                result++;
            }

            registers.SetCarryFlag(result > 255);
            registers.SetZeroFlag(((byte)result) == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            var overflow = ((registers.A ^ result) & (operand ^ result) & 0x80) > 0; //checks if the two operands and the result have differing sign bits.  If they do, the operation resulted in an overflow
            registers.SetOverflowFlag(overflow);

            registers.A = (byte)result;
        }

        private static byte ADC_IMM(IBUS bus, IRegisters registers)
        {
            var operand = AddressingImmediate(bus, registers).Value;
            ADC(operand, registers);
            return 2;
        }

        private static byte ADC_IND_X(IBUS bus, IRegisters registers)
        {
            var operand = AddressingIndirectXWithValue(bus, registers).Value;
            ADC(operand, registers);
            return 6;
        }

        private static byte ADC_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZeroWithValue(bus, registers).Value;
            ADC(operand, registers);
            return 3;
        }

        private static void SBC(byte value, IRegisters registers)
        {
            var operand = (sbyte)~value;
            var result = (sbyte)registers.A + operand;
            if (registers.GetCarryFlag() == true)
            {
                result++;
            }

            registers.SetCarryFlag((sbyte)result >= 0);
            registers.SetZeroFlag(((byte)result) == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            var overflow = ((registers.A ^ result) & (operand ^ result) & 0x80) > 0;
            registers.SetOverflowFlag(overflow);

            registers.A = (byte)result;
        }

        private static byte SBC_IMM(IBUS bus, IRegisters registers)
        {
            var operand = AddressingImmediate(bus, registers).Value;
            SBC(operand, registers);
            return 2;
        }

        private static byte SBC_IND_X(IBUS bus, IRegisters registers)
        {
            var operand = AddressingIndirectXWithValue(bus, registers).Value;
            SBC(operand, registers);
            return 6;
        }

        private static void CMP(byte value, IRegisters registers)
        {
            var tmp = registers.A - value;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.A >= value);
            registers.SetNegativeFlag((tmp & 0x80) > 0);
        }

        private static byte CMP_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            CMP(imm, registers);

            return 2;
        }

        private static byte CMP_IND_X(IBUS bus, IRegisters registers)
        {
            var value = AddressingIndirectXWithValue(bus, registers).Value;
            CMP(value, registers);

            return 6;
        }

        private static byte CMP_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZeroWithValue(bus, registers).Value;
            CMP(operand, registers);
            return 3;
        }

        private static byte CPX_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            var tmp = registers.X - imm;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.X >= imm);
            registers.SetNegativeFlag((tmp & 0x80) > 0);

            return 2;
        }

        private static byte CPY_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            var tmp = registers.Y - imm;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.Y >= imm);
            registers.SetNegativeFlag((tmp & 0x80) > 0);

            return 2;
        }
    }
}
