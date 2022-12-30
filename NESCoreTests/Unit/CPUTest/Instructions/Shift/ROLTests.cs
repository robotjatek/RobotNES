using FluentAssertions;
using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.Shift
{
    public class ROLTests : InstructionTestBase
    {
        [Fact]
        public void ROL_A_shifts_value_left_by_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 4;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(8);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_carry_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_carry_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.SetupAllProperties();
            registers.Object.A = 0x7F;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xFE);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_bit0_to_0_when_carry_flag_is_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(4);
            (registers.Object.A & 0x1).Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_bit0_to_1_when_carry_flag_is_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(5);
            (registers.Object.A & 0x1).Should().Be(1);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_rotates_correctly()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 0x6e;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xdd);
            (registers.Object.A & 0x1).Should().NotBe(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_zero_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x4;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x8);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_zero_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_negative_flag_to_false_when_the_input_bit6_is_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x20;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x40);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_A_sets_negative_flag_to_true_when_the_input_bit6_is_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 0x40;

            var rol = _instructions[Opcodes.ROL_A];
            var cycles = rol(bus.Object, registers.Object);

            (registers.Object.A & 0x80).Should().NotBe(0);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROL_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0x10).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var rol = _instructions[Opcodes.ROL_ZERO];
            var cycles = rol(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 8));

            cycles.Should().Be(5);
        }
    }
}
