using FluentAssertions;

using NESCore;

namespace NESCoreTests.Unit.CPUTest
{
    public class RegisterTests
    {
        private const int StatusRegisterInitialValue = 0x24;

        [Fact]
        public void CarryFlagShouldBeEnabled()
        {
            var sut = new Registers();
            sut.SetCarryFlag(true);
            sut.STATUS.Should().Be(0b00000001 | StatusRegisterInitialValue);
        }

        [Fact]
        public void CarryFlagShouldBeDisabled()
        {
            var sut = new Registers();
            sut.SetCarryFlag(true);
            sut.SetCarryFlag(false);
            sut.STATUS.Should().Be(0b00000000 | StatusRegisterInitialValue);
        }

        [Fact]
        public void ZeroFlagShouldBeEnabled()
        {
            var sut = new Registers();
            sut.SetZeroFlag(true);
            sut.STATUS.Should().Be(0b00000010 | StatusRegisterInitialValue);
        }

        [Fact]
        public void ZeroFlagShouldBeDisabled()
        {
            var sut = new Registers();
            sut.SetZeroFlag(true);
            sut.SetZeroFlag(false);
            sut.STATUS.Should().Be(0b00000000 | StatusRegisterInitialValue);
        }

        [Fact]
        public void InterruptDisableFlagShouldBeEnabled()
        {
            var sut = new Registers();
            sut.SetInterruptDisableFlag(true);
            sut.STATUS.Should().Be(0b00000100 | StatusRegisterInitialValue);
        }

        [Fact]
        public void InterruptDisableFlagShouldBeDisabled()
        {
            var sut = new Registers();
            sut.SetInterruptDisableFlag(true);
            sut.SetInterruptDisableFlag(false);
            sut.STATUS.Should().Be(0b00000000 | (StatusRegisterInitialValue & ~FlagPositions.INTERRUPT_DISABLE));
        }
        [Fact]
        public void DecimalFlagShouldBeEnabled()
        {
            var sut = new Registers();
            sut.SetDecimalFlag(true);
            sut.STATUS.Should().Be(0b00001000 | StatusRegisterInitialValue    );
        }

        [Fact]
        public void DecimalFlagShouldBeDisabled()
        {
            var sut = new Registers();
            sut.SetDecimalFlag(true);
            sut.SetDecimalFlag(false);
            sut.STATUS.Should().Be(0b00000000 | StatusRegisterInitialValue);
        }

        [Fact]
        public void OverflowFlagShouldBeEnabled()
        {
            var sut = new Registers();
            sut.SetOverflowFlag(true);
            sut.STATUS.Should().Be(0b01000000 | StatusRegisterInitialValue);
        }

        [Fact]
        public void OverflowFlagShouldBeDisabled()
        {
            var sut = new Registers();
            sut.SetOverflowFlag(true);
            sut.SetOverflowFlag(false);
            sut.STATUS.Should().Be(0b00000000 | StatusRegisterInitialValue);
        }

        [Fact]
        public void NegativeFlagShouldBeEnabled()
        {
            var sut = new Registers();
            sut.SetNegativeFlag(true);
            sut.STATUS.Should().Be(0b10000000 | StatusRegisterInitialValue);
        }

        [Fact]
        public void NegativeFlagShouldBeDisabled()
        {
            var sut = new Registers();
            sut.SetNegativeFlag(true);
            sut.SetNegativeFlag(false);
            sut.STATUS.Should().Be(0b00000000 | StatusRegisterInitialValue);
        }

    }
}
