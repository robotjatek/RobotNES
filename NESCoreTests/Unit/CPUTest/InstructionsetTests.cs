﻿
using FluentAssertions;

using Moq;

using NESCore;

namespace NESCoreTests.Unit.CPUTest
{
    public class InstructionsetTests
    {
        [Fact]
        public void JMP_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var jmp_abs = new CPUInstructions().InstructionSet[Opcodes.JMP_ABS];
            var cycles = jmp_abs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xdead);
            cycles.Should().Be(3);
        }

        [Fact]
        public void LDA_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0x10);

            var lda_imm = new CPUInstructions().InstructionSet[Opcodes.LDA_IMM];
            var cycles = lda_imm(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(2);

        }

        [Fact]
        public void LDA_IMM_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0);

            var lda_imm = new CPUInstructions().InstructionSet[Opcodes.LDA_IMM];
            var cycles = lda_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDA_IMM_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-1));

            var lda_imm = new CPUInstructions().InstructionSet[Opcodes.LDA_IMM];
            var cycles = lda_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDX_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0x10);

            var ldx_imm = new CPUInstructions().InstructionSet[Opcodes.LDX_IMM];
            var cycles = ldx_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDX_IMM_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0);

            var ldx_imm = new CPUInstructions().InstructionSet[Opcodes.LDX_IMM];
            var cycles = ldx_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDX_IMM_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-1));

            var ldx_imm = new CPUInstructions().InstructionSet[Opcodes.LDX_IMM];
            var cycles = ldx_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void STA_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.A).Returns(10);
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xde);
            var sut = new CPUInstructions().InstructionSet[Opcodes.STA_ZERO];
            var cycles = sut(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x00de, 10));

            //No flags affected
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
        public void STX_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.X).Returns(10);

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xaa);

            var stx_zero = new CPUInstructions().InstructionSet[Opcodes.STX_ZERO];
            var cycles = stx_zero(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xaa, 10));

            //No flags affected
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
        public void JSR_ABS()
        {
            var registersMock = new Mock<IRegisters>();

            var registers = new Mock<Registers>();
            registers.Object.PC = 0xd000;
            registers.Object.STATUS = 0;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var jsr_abs = new CPUInstructions().InstructionSet[Opcodes.JSR_ABS];
            var cycles = jsr_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0x100 | 0xff, 0xd0));
            bus.Verify(b => b.Write(0x100 | 0xfe, 0x01));

            registers.Object.PC.Should().Be((UInt16)0xdead);
            registers.Object.STATUS.Should().Be(0);
            cycles.Should().Be(6);
        }

        [Fact]
        public void RTS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xFF - 2;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(0x100 | 0xff)).Returns(0xde);
            bus.Setup(b => b.Read(0x100 | 0xff - 1)).Returns(0xad);

            var sut = new CPUInstructions().InstructionSet[Opcodes.RTS];
            var cycles = sut(bus.Object, registers.Object);

            registers.Object.PC.Should().Be((UInt16)(0xdead + 1));
            registers.Object.SP.Should().Be(0xFF);

            cycles.Should().Be(6);
        }

        [Fact]
        public void NOP()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            var nop = new CPUInstructions().InstructionSet[Opcodes.NOP];
            var cycles = nop(bus.Object, registers.Object);

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
        public void SEC()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var sec = new CPUInstructions().InstructionSet[Opcodes.SEC];
            var cycles = sec(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once);
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
        public void SEI()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var sei = new CPUInstructions().InstructionSet[Opcodes.SEI];
            var cycles = sei(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Once());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void SED()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var sed = new CPUInstructions().InstructionSet[Opcodes.SED];
            var cycles = sed(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Once());
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-9));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-13));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-9));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-13));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-9));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-13));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-9));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-13));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-9));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-13));
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
        public void CLC()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            var clc = new CPUInstructions().InstructionSet[Opcodes.CLC];
            var cycles = clc(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false));

            //Everything else never happens
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-9));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-13));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-9));
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(10);
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(unchecked((byte)-13));
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
        public void BIT_ZERO_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0x80); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0x40); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0xC0); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0x0); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0xFF); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0xFF); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0x55); //0x0010 address on zero page, then the value
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0xAA); //0x0010 address on zero page, then the value
            var sut = new CPUInstructions().InstructionSet[Opcodes.BIT_ZERO];
            var cycles = sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(false));

            cycles.Should().Be(3);
        }

        [Fact]
        public void PHP()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xff;
            registers.Object.STATUS = 0xAA;

            var bus = new Mock<IBUS>();
            var php = new CPUInstructions().InstructionSet[Opcodes.PHP];
            var cycles = php(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0x1ff, 0xAA));
            registers.Object.SP.Should().Be(0xfe);

            cycles.Should().Be(3);
        }

        [Fact]
        public void PLA_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xfe;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(0x1ff)).Returns(unchecked((byte)-34));

            var pla = new CPUInstructions().InstructionSet[Opcodes.PLA];
            var cycles = pla(bus.Object, registers.Object);

            registers.Object.A.Should().Be(unchecked((byte)-34));
            registers.Object.SP.Should().Be(0xff);

            //Sets negative and zero flags accoringly
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void PLA_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xfe;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(0x1ff)).Returns(0);

            var pla = new CPUInstructions().InstructionSet[Opcodes.PLA];
            var cycles = pla(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Object.SP.Should().Be(0xff);

            //Sets negative and zero flags accoringly
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
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

            cycles.Should().Be(4);
        }

        [Fact]
        public void PLA_does_not_set_flags()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xfe;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(0x1ff)).Returns(10);

            var pla = new CPUInstructions().InstructionSet[Opcodes.PLA];
            var cycles = pla(bus.Object, registers.Object);

            registers.Object.A.Should().Be(10);
            registers.Object.SP.Should().Be(0xff);

            //Sets negative and zero flags accoringly
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(4);
        }

        [Fact]
        public void AND_IMM_does_not_change_A_when_bitmask_is_FF()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x2A;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xff);

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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0);

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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0);

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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xff);

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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xff);

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
        public void CMP_IMM_sets_zero_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = new CPUInstructions().InstructionSet[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = new CPUInstructions().InstructionSet[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_when_a_is_larger_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 27;

            var cmp = new CPUInstructions().InstructionSet[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_to_false_when_a_is_less_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = new CPUInstructions().InstructionSet[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_zero_flag_to_false_when_the_two_numbers_are_not_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = new CPUInstructions().InstructionSet[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_negative_flag_when_operand_is_larger_than_a()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = new CPUInstructions().InstructionSet[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }
    }
}
