using Moq;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class StackInstructionTests : InstructionTestBase
    {
        [Fact]
        public void PHA()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xff;
            registers.Object.A = 0xde;

            var bus = new Mock<IBUS>();
            var pha = _instructions[Opcodes.PHA];
            var cycles = pha(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0x1ff, 0xde));
            registers.Object.SP.Should().Be(0xfe);

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
            var php = _instructions[Opcodes.PHP];
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

            var pla = _instructions[Opcodes.PLA];
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

            var pla = _instructions[Opcodes.PLA];
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

            var pla = _instructions[Opcodes.PLA];
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
        public void PLP()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xfe;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(0x1ff)).Returns(0xff);

            var plp = _instructions[Opcodes.PLP];
            var cycles = plp(bus.Object, registers.Object);

            registers.Object.STATUS.Should().Be(0xef); //BRK flag ignored
            registers.Object.SP.Should().Be(0xff);

            //All flags are set from the value of the stack

            cycles.Should().Be(4);
        }
    }
}
