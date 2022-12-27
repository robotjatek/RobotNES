using Moq;
using NESCore.CPU.Instructions;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LDYTests
    {
        [Fact]
        public void LDY_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x10);

            var ldy_imm = new CPUInstructions().InstructionSet[Opcodes.LDY_IMM];
            var cycles = ldy_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.Y = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDY_IMM_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var ldy_imm = new CPUInstructions().InstructionSet[Opcodes.LDY_IMM];
            var cycles = ldy_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.Y = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDY_IMM_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-1));

            var ldy_imm = new CPUInstructions().InstructionSet[Opcodes.LDY_IMM];
            var cycles = ldy_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.Y = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }
    }
}
