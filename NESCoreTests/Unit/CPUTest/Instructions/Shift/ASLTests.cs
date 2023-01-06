using FluentAssertions;
using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.Shift
{
    public class ASLTests : InstructionTestBase
    {
        [Fact]
        public void ASL_A_shifts_A_by_one_to_the_left()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_A_sets_carry_to_true_when_bit_7_is_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_A_sets_carry_to_false_when_bit_7_is_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7f;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfe);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_A_sets_negative_flag_to_false_when_the_result_is_not_negative()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x1;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x2);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_A_sets_negative_flag_to_true_when_the_result_is_negative()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7F;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfe);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_A_sets_zero_flag_to_false_when_to_result_is_not_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_A_sets_zero_flag_to_true_when_to_result_is_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_A_bit_0_always_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;

            var asl = _instructions[Opcodes.ASL_A];
            var cycles = asl(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfe);
            (registers.Object.A & 0x1).Should().Be(0);

            cycles.Should().Be(2);
        }

        [Fact]
        public void ASL_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0x10).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var asl = _instructions[Opcodes.ASL_ZERO];
            var cycles = asl(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 8));

            cycles.Should().Be(5);
        }

        [Fact]
        public void ASL_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0x10).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var asl = _instructions[Opcodes.ASL_ZERO_X];
            var cycles = asl(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10 + 5, 8));

            cycles.Should().Be(6);
        }

        [Fact]
        public void ASL_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xFF).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var asl = _instructions[Opcodes.ASL_ZERO_X];
            var cycles = asl(bus.Object, registers.Object);
            bus.Verify(b => b.Write(4, 8));

            cycles.Should().Be(6);
        }

        [Fact]
        public void ASL_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var asl = _instructions[Opcodes.ASL_ABS];
            var cycles = asl(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead, 8));

            cycles.Should().Be(6);
        }

        [Fact]
        public void ASL_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var asl = _instructions[Opcodes.ASL_ABS_X];
            var cycles = asl(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead + 10, 8));

            cycles.Should().Be(7);
        }
    }
}
