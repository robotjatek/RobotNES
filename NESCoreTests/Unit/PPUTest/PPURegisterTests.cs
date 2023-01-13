using NESCore.PPU;

namespace NESCoreTests.Unit.PPUTest
{
    public class PPURegisterTests : TestBase
    {
        [Fact]
        public void SetsPPUStatusVBlankFlagToTrue()
        {
            var registers = new PPURegisters
            {
                Status = 0
            };

            registers.SetVBlankFlag(true);
            registers.GetVBlankFlag().Should().BeTrue();
        }

        [Fact]
        public void SetsPPUStatusVBlankFlagToFalse()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            registers.SetVBlankFlag(false);
            registers.GetVBlankFlag().Should().BeFalse();
        }

        [Fact]
        public void SetsPPUStatusOverflowFlagToTrue()
        {
            var registers = new PPURegisters
            {
                Status = 0
            };

            registers.SetOverflowFlag(true);
            registers.GetOverflowFlag().Should().BeTrue();
        }

        [Fact]
        public void SetsPPUStatusOverflowFlagToFalse()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            registers.SetOverflowFlag(false);
            registers.GetOverflowFlag().Should().BeFalse();
        }

        [Fact]
        public void SetsPPUStatusSpriteZeroHitFlagToTrue()
        {
            var registers = new PPURegisters
            {
                Status = 0
            };

            registers.SetSpriteZeroHitFlag(true);
            registers.GetSpriteZeroHitFlag().Should().BeTrue();
        }

        [Fact]
        public void SetsPPUStatusSpriteZeroHitFlagToFalse()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            registers.SetSpriteZeroHitFlag(false);
            registers.GetSpriteZeroHitFlag().Should().BeFalse();
        }
    }
}
