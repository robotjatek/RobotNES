namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
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
    }
}
