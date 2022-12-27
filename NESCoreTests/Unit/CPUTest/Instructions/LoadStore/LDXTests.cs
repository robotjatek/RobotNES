using Moq;
using NESCore.CPU.Instructions;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LDXTests
    {
        [Fact]
        public void LDX_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x10);

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
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

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
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-1));

            var ldx_imm = new CPUInstructions().InstructionSet[Opcodes.LDX_IMM];
            var cycles = ldx_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDX_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var ldx_abs = new CPUInstructions().InstructionSet[Opcodes.LDX_ABS];
            var cycles = ldx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDX_ABS_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x0);

            var ldx_abs = new CPUInstructions().InstructionSet[Opcodes.LDX_ABS];
            var cycles = ldx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDX_ABS_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(unchecked((byte)-1));

            var ldx_abs = new CPUInstructions().InstructionSet[Opcodes.LDX_ABS];
            var cycles = ldx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(4);
        }
    }
}
