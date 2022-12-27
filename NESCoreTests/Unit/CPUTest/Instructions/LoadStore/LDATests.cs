using Moq;
using NESCore.CPU.Instructions;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LDATests
    {
        [Fact]
        public void LDA_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x10);

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
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

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
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-1));

            var lda_imm = new CPUInstructions().InstructionSet[Opcodes.LDA_IMM];
            var cycles = lda_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDA_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var lda_abs = new CPUInstructions().InstructionSet[Opcodes.LDA_ABS];
            var cycles = lda_abs(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xdead));
            cycles.Should().Be(4);

        }

        [Fact]
        public void LDA_ABS_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0);

            var lda_abs = new CPUInstructions().InstructionSet[Opcodes.LDA_ABS];
            var cycles = lda_abs(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            bus.Verify(b => b.Read(0xdead));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDA_ABS_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(unchecked((byte)-1));

            var lda_abs = new CPUInstructions().InstructionSet[Opcodes.LDA_ABS];
            var cycles = lda_abs(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            bus.Verify(b => b.Read(0xdead));
            cycles.Should().Be(4);
        }
    }
}
