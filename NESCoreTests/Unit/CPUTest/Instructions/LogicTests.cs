﻿using Moq;
using NESCore.CPU.Instructions;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class LogicTests
    {
        [Fact]
        public void AND_IMM_does_not_change_A_when_bitmask_is_FF()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x2A;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var and = new CPUInstructions().InstructionSet[Opcodes.AND_IMM];
            registers.Object.A.Should().Be(0x2A);
            var cycles = and(bus.Object, registers.Object);

            cycles.Should().Be(2);
        }

        [Fact]
        public void AND_IMM_zeroes_out_A_when_bitmask_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var and = new CPUInstructions().InstructionSet[Opcodes.AND_IMM];
            var cycles = and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            cycles.Should().Be(2);
        }

        [Fact]
        public void AND_IMM_sets_zero_flag_when_bitmask_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var and = new CPUInstructions().InstructionSet[Opcodes.AND_IMM];
            var cycles = and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void AND_IMM_sets_zero_flag_when_acc_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var and = new CPUInstructions().InstructionSet[Opcodes.AND_IMM];
            var cycles = and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void AND_IMM_sets_negative_flag_when_result_is_negative()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var and = new CPUInstructions().InstructionSet[Opcodes.AND_IMM];
            var cycles = and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BIT_ZERO_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x80); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_overflow_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x40); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetOverflowFlag(true));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_both_negative_and_overflow_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xC0); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));
            registers.Verify(r => r.SetOverflowFlag(true));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_neither_negative_and_overflow_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x0); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetOverflowFlag(false));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_when_A_is_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xFF); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));
            registers.Verify(r => r.SetOverflowFlag(true));
            registers.Verify(r => r.SetZeroFlag(true));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_when_value_is_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xFF);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(true));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_to_false_when_A_is_not_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xFF);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xFF); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(false));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_to_true_when_bit_patterns_do_not_match()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xAA);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x55); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(true));

            cycles.Should().Be(3);
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_to_false_when_bit_patterns_match()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xAA);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xAA); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(false));

            cycles.Should().Be(3);
        }

        [Fact]
        public void EOR_IMM_does_not_change_A_when_bitmask_is_00()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x2A;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x00);

            var eor = new CPUInstructions().InstructionSet[Opcodes.EOR_IMM];
            registers.Object.A.Should().Be(0x2A);
            var cycles = eor(bus.Object, registers.Object);

            cycles.Should().Be(2);
        }

        [Fact]
        public void EOR_IMM_zeroes_out_A_when_bitmask_is_the_same()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xde;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xde);

            var eor = new CPUInstructions().InstructionSet[Opcodes.EOR_IMM];
            var cycles = eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            cycles.Should().Be(2);
        }

        [Fact]
        public void AND_IMM_sets_zero_flag_when_bitmask_is_the_same()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xFF);

            var eor = new CPUInstructions().InstructionSet[Opcodes.EOR_IMM];
            var cycles = eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void EOR_IMM_sets_negative_flag_when_result_is_negative()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x00;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var eor = new CPUInstructions().InstructionSet[Opcodes.EOR_IMM];
            var cycles = eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void EOR_IMM_sets_the_correct_value()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x55);

            var eor = new CPUInstructions().InstructionSet[Opcodes.EOR_IMM];
            var cycles = eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ORA_IMM_does_not_change_A_when_bitmask_is_00()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x2A;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x00);

            var ora = new CPUInstructions().InstructionSet[Opcodes.ORA_IMM];
            registers.Object.A.Should().Be(0x2A);
            var cycles = ora(bus.Object, registers.Object);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ORA_IMM_fills_A_with_ones_when_bitmask_is_FF()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var ora = new CPUInstructions().InstructionSet[Opcodes.ORA_IMM];
            var cycles = ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ORA_IMM_sets_zero_flag_when_result_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var ora = new CPUInstructions().InstructionSet[Opcodes.ORA_IMM];
            var cycles = ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ORA_IMM_sets_negative_flag_when_result_is_negative()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var ora = new CPUInstructions().InstructionSet[Opcodes.ORA_IMM];
            var cycles = ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }
    }
}