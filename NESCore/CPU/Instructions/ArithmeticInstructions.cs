namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
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

        private static void ADC_IMM(IBUS bus, IRegisters registers)
        {
            var operand = AddressingImmediate(bus, registers).Value;
            ADC(operand, registers);
        }

        private static void ADC_IND_X(IBUS bus, IRegisters registers)
        {
            var operand = AddressingIndirectX(bus, registers).Value;
            ADC(operand, registers);
        }

        private static void ADC_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZero(bus, registers).Value;
            ADC(operand, registers);
        }

        private static void ADC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            ADC(addressingResult.Value, registers);
        }

        private static void ADC_ABS(IBUS bus, IRegisters registers)
        {
            var operand = AddressingAbsolute(bus, registers).Value;
            ADC(operand, registers);
        }

        private static void ADC_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            ADC(addressingResult.Value, registers);
        }

        private static void ADC_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            ADC(addressingResult.Value, registers);
        }

        private static void ADC_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            ADC(addressingResult.Value, registers);
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

        private static void SBC_IMM(IBUS bus, IRegisters registers)
        {
            var operand = AddressingImmediate(bus, registers).Value;
            SBC(operand, registers);
        }

        private static void SBC_IND_X(IBUS bus, IRegisters registers)
        {
            var operand = AddressingIndirectX(bus, registers).Value;
            SBC(operand, registers);
        }

        private void SBC_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZero(bus, registers).Value;
            SBC(operand, registers);
        }

        private void SBC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            SBC(addressingResult.Value, registers);
        }

        private void SBC_ABS(IBUS bus, IRegisters registers)
        {
            var operand = AddressingAbsolute(bus, registers).Value;
            SBC(operand, registers);
        }

        private static void SBC_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            SBC(addressingResult.Value, registers);
        }

        private static void SBC_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            SBC(addressingResult.Value, registers);
        }

        private void SBC_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            SBC(addressingResult.Value, registers);
        }

        private static void CMP(byte value, IRegisters registers)
        {
            var tmp = registers.A - value;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.A >= value);
            registers.SetNegativeFlag((tmp & 0x80) > 0);
        }

        private static void CMP_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            CMP(imm, registers);
        }

        private static void CMP_IND_X(IBUS bus, IRegisters registers)
        {
            var value = AddressingIndirectX(bus, registers).Value;
            CMP(value, registers);
        }

        private static void CMP_ZERO(IBUS bus, IRegisters registers)
        {
            var operand = AddressingZero(bus, registers).Value;
            CMP(operand, registers);
        }

        private static void CMP_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            CMP(addressingResult.Value, registers);
        }

        private static void CMP_ABS(IBUS bus, IRegisters registers)
        {
            var operand = AddressingAbsolute(bus, registers).Value;
            CMP(operand, registers);
        }

        private static void CMP_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            CMP(addressingResult.Value, registers);
        }

        private static void CMP_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            CMP(addressingResult.Value, registers);
        }

        private static void CMP_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            CMP(addressingResult.Value, registers);
        }

        private static void CPX(byte value, IRegisters registers)
        {
            var tmp = registers.X - value;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.X >= value);
            registers.SetNegativeFlag((tmp & 0x80) > 0);
        }

        private static void CPX_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            CPX(imm, registers);
        }

        private static void CPX_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZero(bus, registers).Value;
            CPX(value, registers);
        }

        private static void CPX_ABS(IBUS bus, IRegisters registers)
        {
            var value = AddressingAbsolute(bus, registers).Value;
            CPX(value, registers);
        }

        private static void CPY(byte value, IRegisters registers)
        {
            var tmp = registers.Y - value;

            registers.SetZeroFlag(tmp == 0);
            registers.SetCarryFlag(registers.Y >= value);
            registers.SetNegativeFlag((tmp & 0x80) > 0);
        }

        private static void CPY_IMM(IBUS bus, IRegisters registers)
        {
            var imm = AddressingImmediate(bus, registers).Value;
            CPY(imm, registers);
        }

        private static void CPY_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZero(bus, registers).Value;
            CPY(value, registers);
        }

        private static void CPY_ABS(IBUS bus, IRegisters registers)
        {
            var value = AddressingAbsolute(bus, registers).Value;
            CPY(value, registers);
        }

        private static void DCP(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var decResult = addressingResult.ReadModifyWrite(bus, value => DEC(value, registers));
            bus.Write(addressingResult.Address, decResult);
            CMP(decResult, registers);
        }

        private static void DCP_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            DCP(addressingResult, bus, registers);
        }

        private static void DCP_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            DCP(addressingResult, bus, registers);
        }

        private static void DCP_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            DCP(addressingResult, bus, registers);
        }

        private static void DCP_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            DCP(addressingResult, bus, registers);
        }

        private static void DCP_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            DCP(addressingResult, bus, registers);
        }

        private static void DCP_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            DCP(addressingResult, bus, registers);
        }

        private static void DCP_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            DCP(addressingResult, bus, registers);
        }

        private static void ISB(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var incResult = addressingResult.ReadModifyWrite(bus, value => INC(value, registers));
            bus.Write(addressingResult.Address, incResult);
            SBC(incResult, registers);
        }

        private static void ISB_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            ISB(addressingResult, bus, registers);
        }

        private static void ISB_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            ISB(addressingResult, bus, registers);
        }

        private static void ISB_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            ISB(addressingResult, bus, registers);
        }

        private static void ISB_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            ISB(addressingResult, bus, registers);
            
        }

        private static void ISB_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            ISB(addressingResult, bus, registers);
        }

        private static void ISB_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            ISB(addressingResult, bus, registers);
        }

        private static void ISB_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            ISB(addressingResult, bus, registers);
        }

        private static void SLO(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var aslResult = addressingResult.ReadModifyWrite(bus, value => ASL(value, registers));
            bus.Write(addressingResult.Address, aslResult);
            ORA(aslResult, registers);
        }

        private static void SLO_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            SLO(addressingResult, bus, registers);
        }

        private static void SLO_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            SLO(addressingResult, bus, registers);
        }

        private static void SLO_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            SLO(addressingResult, bus, registers);
        }

        private static void SLO_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            SLO(addressingResult, bus, registers);
        }

        private static void SLO_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            SLO(addressingResult, bus, registers);
        }

        private static void SLO_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            SLO(addressingResult, bus, registers);
        }

        private static void SLO_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            SLO(addressingResult, bus, registers);
        }

        private static void RLA(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var rlaResult = addressingResult.ReadModifyWrite(bus, value => ROL(value, registers));
            bus.Write(addressingResult.Address, rlaResult);
            AND(rlaResult, registers);
        }

        private static void RLA_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            RLA(addressingResult, bus, registers);
        }

        private static void RLA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            RLA(addressingResult, bus, registers);
        }

        private static void RLA_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            RLA(addressingResult, bus, registers);
        }

        private static void RLA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            RLA(addressingResult, bus, registers);
        }

        private static void RLA_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            RLA(addressingResult, bus, registers);
        }

        private static void RLA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            RLA(addressingResult, bus, registers);
        }

        private static void RLA_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            RLA(addressingResult, bus, registers);
        }

        private static void SRE(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var sreResult = addressingResult.ReadModifyWrite(bus, value => LSR(value, registers));
            bus.Write(addressingResult.Address, sreResult);
            EOR(sreResult, registers);
        }

        private static void SRE_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            SRE(addressingResult, bus, registers);
        }

        private static void SRE_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            SRE(addressingResult, bus, registers);
        }

        private static void SRE_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            SRE(addressingResult, bus, registers);
        }

        private static void SRE_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            SRE(addressingResult, bus, registers);
        }

        private static void SRE_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            SRE(addressingResult, bus, registers);
        }

        private static void SRE_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            SRE(addressingResult, bus, registers);
        }

        private static void SRE_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            SRE(addressingResult, bus, registers);
        }

        private static void RRA(AddressingResult addressingResult, IBUS bus, IRegisters registers)
        {
            var rraResult = addressingResult.ReadModifyWrite(bus, value => ROR(value, registers));
            bus.Write(addressingResult.Address, rraResult);
            ADC(rraResult, registers);
        }

        private static void RRA_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            RRA(addressingResult, bus, registers);
        }

        private static void RRA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            RRA(addressingResult, bus, registers);
        }

        private static void RRA_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            RRA(addressingResult, bus, registers);
        }

        private static void RRA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            RRA(addressingResult, bus, registers);
        }

        private static void RRA_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            RRA(addressingResult, bus, registers);
        }

        private static void RRA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            RRA(addressingResult, bus, registers);
        }

        private static void RRA_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            RRA(addressingResult, bus, registers);
        }
    }
}
