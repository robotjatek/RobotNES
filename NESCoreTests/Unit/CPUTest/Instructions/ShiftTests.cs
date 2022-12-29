using FluentAssertions;

using Moq;

using NESCore;
using NESCore.CPU;
using NESCore.CPU.Instructions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class ShiftTests : InstructionTestBase
    {
        [Fact]
        public void LSR_A_shifts_value_to_right_by_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 8;

            var lsr = _instructions[Opcodes.LSR_A];
            var cycles = lsr(bus.Object, registers.Object);

            registers.Object.A.Should().Be(4);

            cycles.Should().Be(2);
        }

        [Fact]
        public void LSR_A_sets_the_carry_bit_to_one_when_the_last_bit_was_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 1;

            var lsr = _instructions[Opcodes.LSR_A];
            var cycles = lsr(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void LSR_A_sets_the_carry_flag_to_zero_when_the_last_bit_is_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var lsr = _instructions[Opcodes.LSR_A];
            var cycles = lsr(bus.Object, registers.Object);

            registers.Object.A.Should().Be(1);
            registers.Verify(r => r.SetCarryFlag(false));

            cycles.Should().Be(2);
        }

        [Fact]
        public void LSR_A_result_always_non_negative()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;

            var lsr = _instructions[Opcodes.LSR_A];
            var cycles = lsr(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x7F);
            registers.Verify(r => r.SetNegativeFlag(false));
            (registers.Object.A & 0x80).Should().Be(0);

            cycles.Should().Be(2);
        }

        [Fact]
        public void LSR_A_sets_the_zero_flag_to_false_when_the_result_is_not_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;

            var lsr = _instructions[Opcodes.LSR_A];
            var cycles = lsr(bus.Object, registers.Object);
            registers.Verify(r => r.SetZeroFlag(false));

            cycles.Should().Be(2);
        }

        [Fact]
        public void LSR_A_sets_the_zero_flag_to_true_when_the_result_is_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x1;

            var lsr = _instructions[Opcodes.LSR_A];
            var cycles = lsr(bus.Object, registers.Object);
            registers.Verify(r => r.SetZeroFlag(true));

            cycles.Should().Be(2);
        }

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
    }
}
