using Moq;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class STXTests : InstructionTestBase
    {
        [Fact]
        public void STX_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.X).Returns(10);

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xaa);

            var stx_zero = _instructions[Opcodes.STX_ZERO];
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
        public void STX_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.X).Returns(10);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var stx_abs = _instructions[Opcodes.STX_ABS];
            var cycles = stx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xdead, 10));

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

            cycles.Should().Be(4);
        }

        [Fact]
        public void STX_ZERO_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xad);

            var stx_zero_y = _instructions[Opcodes.STX_ZERO_Y];
            var cycles = stx_zero_y(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xad + 15, 10));

            cycles.Should().Be(4);
        }
    }
}
