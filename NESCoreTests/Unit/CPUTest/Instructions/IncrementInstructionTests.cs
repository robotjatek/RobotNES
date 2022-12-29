using Moq;
using NESCore.CPU.Instructions;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class IncrementInstructionTests : InstructionTestBase
    {
        [Fact]
        public void DEX_decrements_Y()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 2;

            var dex = _instructions[Opcodes.DEX];
            var cycles = dex(bus.Object, registers.Object);
            registers.Object.X.Should().Be(1);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEX_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0x1;

            var dex = _instructions[Opcodes.DEX];
            var cycles = dex(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEX_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0x0;

            var dex = _instructions[Opcodes.DEX];
            var cycles = dex(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0xff);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEY_decrements_Y()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 2;

            var dey = _instructions[Opcodes.DEY];
            var cycles = dey(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(1);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEY_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0x1;

            var dey = _instructions[Opcodes.DEY];
            var cycles = dey(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEY_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0x0;

            var dey = _instructions[Opcodes.DEY];
            var cycles = dey(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0xff);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INX_increments_X()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 1;

            var inx = _instructions[Opcodes.INX];
            var cycles = inx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(2);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INX_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0xff;

            var inx = _instructions[Opcodes.INX];
            var cycles = inx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INX_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0x7F;

            var inx = _instructions[Opcodes.INX];
            var cycles = inx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INY_increments_Y()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 1;

            var iny = _instructions[Opcodes.INY];
            var cycles = iny(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(2);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INY_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0xff;

            var iny = _instructions[Opcodes.INY];
            var cycles = iny(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INY_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0x7F;

            var iny = _instructions[Opcodes.INY];
            var cycles = iny(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }
    }
}
