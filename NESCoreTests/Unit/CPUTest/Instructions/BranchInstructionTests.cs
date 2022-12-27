﻿using Moq;
using NESCore.CPU.Instructions;
using NESCore.CPU;
using NESCore;

using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class BranchInstructionTests
    {
        [Fact]
        public void BCC_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var bcc = new CPUInstructions().InstructionSet[Opcodes.BCC];
            var cycles = bcc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BCC_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var bcc = new CPUInstructions().InstructionSet[Opcodes.BCC];
            var cycles = bcc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BCC_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var bcc = new CPUInstructions().InstructionSet[Opcodes.BCC];
            var cycles = bcc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BCC_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var bcc = new CPUInstructions().InstructionSet[Opcodes.BCC];
            var cycles = bcc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BCC_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var bcc = new CPUInstructions().InstructionSet[Opcodes.BCC];
            var cycles = bcc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BCS_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;

            var bcs = new CPUInstructions().InstructionSet[Opcodes.BCS];
            var cycles = bcs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BCS_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var bcs = new CPUInstructions().InstructionSet[Opcodes.BCS];
            var cycles = bcs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BCS_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var bcs = new CPUInstructions().InstructionSet[Opcodes.BCS];
            var cycles = bcs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BCS_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var bcs = new CPUInstructions().InstructionSet[Opcodes.BCS];
            var cycles = bcs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BCS_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var bcs = new CPUInstructions().InstructionSet[Opcodes.BCS];
            var cycles = bcs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BEQ_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;

            var beq = new CPUInstructions().InstructionSet[Opcodes.BEQ];
            var cycles = beq(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BEQ_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetZeroFlag()).Returns(true);

            var beq = new CPUInstructions().InstructionSet[Opcodes.BEQ];
            var cycles = beq(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BEQ_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetZeroFlag()).Returns(true);

            var beq = new CPUInstructions().InstructionSet[Opcodes.BEQ];
            var cycles = beq(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BEQ_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetZeroFlag()).Returns(true);

            var beq = new CPUInstructions().InstructionSet[Opcodes.BEQ];
            var cycles = beq(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BEQ_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetZeroFlag()).Returns(true);

            var beq = new CPUInstructions().InstructionSet[Opcodes.BEQ];
            var cycles = beq(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BMI_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);

            var bmi = new CPUInstructions().InstructionSet[Opcodes.BMI];
            var cycles = bmi(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BMI_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetNegativeFlag()).Returns(true);

            var bmi = new CPUInstructions().InstructionSet[Opcodes.BMI];
            var cycles = bmi(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BMI_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetNegativeFlag()).Returns(true);

            var bmi = new CPUInstructions().InstructionSet[Opcodes.BMI];
            var cycles = bmi(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BMI_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetNegativeFlag()).Returns(true);

            var bmi = new CPUInstructions().InstructionSet[Opcodes.BMI];
            var cycles = bmi(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BMI_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetNegativeFlag()).Returns(true);

            var bmi = new CPUInstructions().InstructionSet[Opcodes.BMI];
            var cycles = bmi(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BNE_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;
            registers.Setup(r => r.GetZeroFlag()).Returns(true);

            var bne = new CPUInstructions().InstructionSet[Opcodes.BNE];
            var cycles = bne(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BNE_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetZeroFlag()).Returns(false);

            var bne = new CPUInstructions().InstructionSet[Opcodes.BNE];
            var cycles = bne(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BNE_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetZeroFlag()).Returns(false);

            var bne = new CPUInstructions().InstructionSet[Opcodes.BNE];
            var cycles = bne(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BNE_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetZeroFlag()).Returns(false);

            var bne = new CPUInstructions().InstructionSet[Opcodes.BNE];
            var cycles = bne(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BNE_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetZeroFlag()).Returns(false);

            var bne = new CPUInstructions().InstructionSet[Opcodes.BNE];
            var cycles = bne(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BPL_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;
            registers.Setup(r => r.GetNegativeFlag()).Returns(true);

            var bpl = new CPUInstructions().InstructionSet[Opcodes.BPL];
            var cycles = bpl(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BPL_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);

            var bpl = new CPUInstructions().InstructionSet[Opcodes.BPL];
            var cycles = bpl(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BPL_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);

            var bpl = new CPUInstructions().InstructionSet[Opcodes.BPL];
            var cycles = bpl(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BPL_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);

            var bpl = new CPUInstructions().InstructionSet[Opcodes.BPL];
            var cycles = bpl(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BPL_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);

            var bpl = new CPUInstructions().InstructionSet[Opcodes.BPL];
            var cycles = bpl(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BVC_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;
            registers.Setup(r => r.GetOverflowFlag()).Returns(true);

            var bvc = new CPUInstructions().InstructionSet[Opcodes.BVC];
            var cycles = bvc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BVC_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetOverflowFlag()).Returns(false);

            var bvc = new CPUInstructions().InstructionSet[Opcodes.BVC];
            var cycles = bvc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BVC_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetOverflowFlag()).Returns(false);

            var bvc = new CPUInstructions().InstructionSet[Opcodes.BVC];
            var cycles = bvc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BVC_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetOverflowFlag()).Returns(false);

            var bvc = new CPUInstructions().InstructionSet[Opcodes.BVC];
            var cycles = bvc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BVC_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetOverflowFlag()).Returns(false);

            var bvc = new CPUInstructions().InstructionSet[Opcodes.BVC];
            var cycles = bvc(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BVS_doesNotTakeTheBranch()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0;

            var bvs = new CPUInstructions().InstructionSet[Opcodes.BVS];
            var cycles = bvs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 1);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void BVS_TakesTheBranchForwardSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetOverflowFlag()).Returns(true);

            var bvs = new CPUInstructions().InstructionSet[Opcodes.BVS];
            var cycles = bvs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 11);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BVS_TakesTheBranchBackwardsSamePage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-9));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 10;
            registers.Setup(r => r.GetOverflowFlag()).Returns(true);

            var bvs = new CPUInstructions().InstructionSet[Opcodes.BVS];
            var cycles = bvs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 2);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(3);
        }

        [Fact]
        public void BVS_TakesTheBranchForwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xfe;
            registers.Setup(r => r.GetOverflowFlag()).Returns(true);

            var bvs = new CPUInstructions().InstructionSet[Opcodes.BVS];
            var cycles = bvs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 265);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void BVS_TakesTheBranchBackwardOtherPage()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-13));
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0x100;
            registers.Setup(r => r.GetOverflowFlag()).Returns(true);

            var bvs = new CPUInstructions().InstructionSet[Opcodes.BVS];
            var cycles = bvs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xf4);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }
    }
}