using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class STYTests : InstructionTestBase
    {
        public STYTests(InstructionsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void STY_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.Y).Returns(10);

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xaa);

            var sty_zero = _instructions[Opcodes.STY_ZERO];
            var cycles = sty_zero(bus.Object, registers.Object);

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
        public void STY_ZERO_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xaa);

            var sty = _instructions[Opcodes.STY_ZERO_X];
            var cycles = sty(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xaa + 10, 5));

            cycles.Should().Be(4);
        }

        [Fact]
        public void STY_ZERO_X_Wraps()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var sty = _instructions[Opcodes.STY_ZERO_X];
            var cycles = sty(bus.Object, registers.Object);

            bus.Verify(b => b.Write(9, 5));

            cycles.Should().Be(4);
        }

        [Fact]
        public void STY_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.Y).Returns(10);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var sty_abs = _instructions[Opcodes.STY_ABS];
            var cycles = sty_abs(bus.Object, registers.Object);

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
    }
}
