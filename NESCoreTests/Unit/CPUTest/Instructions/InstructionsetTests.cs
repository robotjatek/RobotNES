using FluentAssertions;

using Moq;

using NESCore;
using NESCore.CPU;
using NESCore.CPU.Instructions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class InstructionsetTests : InstructionTestBase
    {
        [Fact]
        public void NOP()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            var nop = _instructions[Opcodes.NOP];
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
    }
}
