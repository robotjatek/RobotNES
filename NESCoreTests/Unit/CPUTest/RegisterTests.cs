using NESCore.CPU;

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
            sut.STATUS.Should().Be(0b00000000 | StatusRegisterInitialValue & ~FlagPositions.INTERRUPT_DISABLE);
        }
        [Fact]
        public void DecimalFlagShouldBeEnabled()
        {
            var sut = new Registers();
            sut.SetDecimalFlag(true);
            sut.STATUS.Should().Be(0b00001000 | StatusRegisterInitialValue);
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

        [Fact]
        public void Bit5AlwaysTrue()
        {
            var sut = new Registers();
            sut.STATUS = 0;
            sut.STATUS.Should().Be(0x20);
        }

        [Fact]
        public void GetCarryFlag()
        {
            var registers = new Registers();
            registers.STATUS = 0b00000001;
            registers.GetCarryFlag().Should().BeTrue();

            registers.STATUS = 0x00;
            registers.GetCarryFlag().Should().BeFalse();
        }

        [Fact]
        public void GetZeroFlag()
        {
            var registers = new Registers();
            registers.STATUS = 0b00000010;
            registers.GetZeroFlag().Should().BeTrue();

            registers.STATUS = 0x00;
            registers.GetZeroFlag().Should().BeFalse();
        }

        [Fact]
        public void GetInterruptDisableFlag()
        {
            var registers = new Registers();
            registers.STATUS = 0b00000100;
            registers.GetInterruptDisableFlag().Should().BeTrue();

            registers.STATUS = 0x00;
            registers.GetInterruptDisableFlag().Should().BeFalse();
        }

        [Fact]
        public void GetDecimalFlag()
        {
            var registers = new Registers();
            registers.STATUS = 0b00001000;
            registers.GetDecimalFlag().Should().BeTrue();

            registers.STATUS = 0x00;
            registers.GetDecimalFlag().Should().BeFalse();
        }


        [Fact]
        public void GetOverflowFlag()
        {
            var registers = new Registers();
            registers.STATUS = 0b01000000;
            registers.GetOverflowFlag().Should().BeTrue();

            registers.STATUS = 0x00;
            registers.GetOverflowFlag().Should().BeFalse();
        }

        [Fact]
        public void GetNegativeFlag()
        {
            var registers = new Registers();
            registers.STATUS = 0b10000000;
            registers.GetNegativeFlag().Should().BeTrue();

            registers.STATUS = 0x00;
            registers.GetNegativeFlag().Should().BeFalse();
        }
    }
}
