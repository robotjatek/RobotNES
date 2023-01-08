using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions.Shift
{
    public class RORTests : InstructionTestBase
    {
        public RORTests(InstructionsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void ROR_A_shifts_value_right_by_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 4;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(2);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_carry_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 1;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_carry_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(1);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_bit7_to_0_when_carry_flag_is_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(1);
            (registers.Object.A & 0x80).Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_bit7_to_1_when_carry_flag_is_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x81);
            (registers.Object.A & 0x80).Should().NotBe(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_rotates_correctly()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 0x6e;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xb7);
            (registers.Object.A & 0x80).Should().NotBe(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_zero_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x4;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x2);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_zero_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x1;

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_negative_flag_to_false_when_the_input_carry_is_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Verify(r => r.SetNegativeFlag(false), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_A_sets_negative_flag_to_true_when_the_input_carry_is_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();

            var ror = _instructions[Opcodes.ROR_A];
            var cycles = ror(bus.Object, registers.Object);

            registers.Verify(r => r.SetNegativeFlag(true), Times.Once);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ROR_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0x10).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var ror = _instructions[Opcodes.ROR_ZERO];
            var cycles = ror(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 2));

            cycles.Should().Be(5);
        }

        [Fact]
        public void ROR_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0x10).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var ror = _instructions[Opcodes.ROR_ZERO_X];
            var cycles = ror(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10 + 5, 2));

            cycles.Should().Be(6);
        }

        [Fact]
        public void ROR_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var ror = _instructions[Opcodes.ROR_ZERO_X];
            var cycles = ror(bus.Object, registers.Object);
            bus.Verify(b => b.Write(4, 2));

            cycles.Should().Be(6);
        }

        [Fact]
        public void ROR_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var ror = _instructions[Opcodes.ROR_ABS];
            var cycles = ror(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead, 2));

            cycles.Should().Be(6);
        }

        [Fact]
        public void ROR_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var ror = _instructions[Opcodes.ROR_ABS_X];
            var cycles = ror(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead + 10, 2));

            cycles.Should().Be(7);
        }
    }
}
