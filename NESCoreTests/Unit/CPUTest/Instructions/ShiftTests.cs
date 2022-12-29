using FluentAssertions;

using Moq;

using NESCore;
using NESCore.CPU;
using NESCore.CPU.Instructions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class ShiftTests
    {
        [Fact]
        public void LSR_A_shifts_value_to_right_by_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 8;

            var lsr = new CPUInstructions().InstructionSet[Opcodes.LSR_A];
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

            var lsr = new CPUInstructions().InstructionSet[Opcodes.LSR_A];
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

            var lsr = new CPUInstructions().InstructionSet[Opcodes.LSR_A];
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

            var lsr = new CPUInstructions().InstructionSet[Opcodes.LSR_A];
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

            var lsr = new CPUInstructions().InstructionSet[Opcodes.LSR_A];
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

            var lsr = new CPUInstructions().InstructionSet[Opcodes.LSR_A];
            var cycles = lsr(bus.Object, registers.Object);
            registers.Verify(r => r.SetZeroFlag(true));

            cycles.Should().Be(2);
        }
    }
}
