using System.Security.Cryptography.X509Certificates;

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
            var operand = AddressingIndirectX(bus, registers).Value;
            ADC(operand, registers);
            return 6;
        }

        private static byte ADC_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZero(bus, registers).Value;
            ADC(operand, registers);
            return 3;
        }

        private static byte ADC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            ADC(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte ADC_ABS(IBUS bus, IRegisters registers)
        {
            var operand = AddressingAbsolute(bus, registers).Value;
            ADC(operand, registers);
            return 4;
        }

        private static byte ADC_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            ADC(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte ADC_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            ADC(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte ADC_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            ADC(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static void SBC(byte value, IRegisters registers)
        {
            var operand = ~value;
            var result = registers.A + operand;
            if (registers.GetCarryFlag() == true)
            {
                result++;
            }

            registers.SetCarryFlag(result >= 0);
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
            var operand = AddressingIndirectX(bus, registers).Value;
            SBC(operand, registers);
            return 6;
        }

        private byte SBC_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZero(bus, registers).Value;
            SBC(operand, registers);
            return 3;
        }

        private byte SBC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            SBC(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private byte SBC_ABS(IBUS bus, IRegisters registers)
        {
            var operand = AddressingAbsolute(bus, registers).Value;
            SBC(operand, registers);
            return 4;
        }

        private static byte SBC_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            SBC(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte SBC_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            SBC(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private byte SBC_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            SBC(addressingResult.Value, registers);
            return addressingResult.Cycles;
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
            var value = AddressingIndirectX(bus, registers).Value;
            CMP(value, registers);

            return 6;
        }

        private static byte CMP_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZero(bus, registers).Value;
            CMP(operand, registers);
            return 3;
        }

        private static byte CMP_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            CMP(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte CMP_ABS(IBUS bus, IRegisters registers)
        {
            var operand = AddressingAbsolute(bus, registers).Value;
            CMP(operand, registers);
            return 4;
        }

        private static byte CMP_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            CMP(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte CMP_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            CMP(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte CMP_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            CMP(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static void CPX(byte value, IRegisters registers)
        {
            var tmp = registers.X - value;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.X >= value);
            registers.SetNegativeFlag((tmp & 0x80) > 0);
        }

        private static byte CPX_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            CPX(imm, registers);

            return 2;
        }

        private static byte CPX_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZero(bus, registers).Value;
            CPX(value, registers);

            return 3;
        }

        private static byte CPX_ABS(IBUS bus, IRegisters registers)
        {
            var value = AddressingAbsolute(bus, registers).Value;
            CPX(value, registers);

            return 4;
        }

        private static void CPY(byte value, IRegisters registers)
        {
            var tmp = registers.Y - value;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.Y >= value);
            registers.SetNegativeFlag((tmp & 0x80) > 0);
        }

        private static byte CPY_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            CPY(imm, registers);

            return 2;
        }

        private static byte CPY_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZero(bus, registers).Value;
            CPY(value, registers);

            return 3;
        }

        private static byte CPY_ABS(IBUS bus, IRegisters registers)
        {
            var value = AddressingAbsolute(bus, registers).Value;
            CPY(value, registers);

            return 4;
        }

        private static void DCP(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var decResult = DEC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, decResult);
            CMP(decResult, registers);
        }

        private static byte DCP_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            DCP(addressingResult, bus, registers);

            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte DCP_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            DCP(addressingResult, bus, registers);

            return 8; //8 regardless of page crossing
        }

        private static byte DCP_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            DCP(addressingResult, bus, registers);

            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte DCP_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            DCP(addressingResult, bus, registers);

            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte DCP_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            DCP(addressingResult, bus, registers);

            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte DCP_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            DCP(addressingResult, bus, registers);

            return 7; // 7 regardless of boundary cross
        }

        private static byte DCP_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            DCP(addressingResult, bus, registers);

            return 7; // 7 regardless of boundary cross
        }

        private static void ISB(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var incResult = INC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, incResult);
            SBC(incResult, registers);
        }

        private static byte ISB_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            ISB(addressingResult, bus, registers);

            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte ISB_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            ISB(addressingResult, bus, registers);

            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte ISB_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            ISB(addressingResult, bus, registers);

            return (byte)(addressingResult.Cycles + 2);
        }
    }
}
