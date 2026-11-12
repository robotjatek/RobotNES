using NESCore.PPU;

namespace NESCoreTests.Unit.PPUTest
{
    public class PpuRegisterTests : TestBase
    {
        [Fact]
        public void SetsPpuStatusVBlankFlagToTrue()
        {
            var registers = new PPURegisters
            {
                Status = 0
            };

            registers.SetVBlankFlag(true);
            registers.GetVBlankFlag().Should().BeTrue();
        }

        [Fact]
        public void SetsPpuStatusVBlankFlagToFalse()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            registers.SetVBlankFlag(false);
            registers.GetVBlankFlag().Should().BeFalse();
        }

        [Fact]
        public void SetsPpuStatusOverflowFlagToTrue()
        {
            var registers = new PPURegisters
            {
                Status = 0
            };

            registers.SetOverflowFlag(true);
            registers.GetOverflowFlag().Should().BeTrue();
        }

        [Fact]
        public void SetsPpuStatusOverflowFlagToFalse()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            registers.SetOverflowFlag(false);
            registers.GetOverflowFlag().Should().BeFalse();
        }

        [Fact]
        public void SetsPpuStatusSpriteZeroHitFlagToTrue()
        {
            var registers = new PPURegisters
            {
                Status = 0
            };

            registers.SetSpriteZeroHitFlag(true);
            registers.GetSpriteZeroHitFlag().Should().BeTrue();
        }

        [Fact]
        public void SetsPpuStatusSpriteZeroHitFlagToFalse()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            registers.SetSpriteZeroHitFlag(false);
            registers.GetSpriteZeroHitFlag().Should().BeFalse();
        }

        [Fact]
        public void VramIncrementMode0()
        {
            var registers = new PPURegisters()
            {
                Control = 0
            };

            registers.VRAMIncrement().Should().Be(1);
        }

        [Fact]
        public void VramIncrementMode1()
        {
            var registers = new PPURegisters()
            {
                Control = 0xff
            };

            registers.VRAMIncrement().Should().Be(32);
        }

        [Fact]
        public void PatternTableAddress0()
        {
            var registers = new PPURegisters()
            {
                Control = 0
            };

            registers.GetSpritePatternTableAddress().Should().Be(0);
        }

        [Fact]
        public void PatternTableAddress1()
        {
            var registers = new PPURegisters()
            {
                Control = 8
            };

            registers.GetSpritePatternTableAddress().Should().Be(0x1000);
        }

        [Fact]
        public void BackgroundTableAddress0()
        {
            var registers = new PPURegisters()
            {
                Control = 0
            };

            registers.GetbackgroundPatternTableAddress().Should().Be(0);
        }

        [Fact]
        public void BackgroundTableAddress1()
        {
            var registers = new PPURegisters()
            {
                Control = 0b10000
            };

            registers.GetbackgroundPatternTableAddress().Should().Be(0x1000);
        }

        [Fact]
        public void SpriteSize0()
        {
            var registers = new PPURegisters()
            {
                Control = 0
            };

            registers.GetSpriteSize().Should().Be(0);
        }

        [Fact]
        public void SpriteSize1()
        {
            var registers = new PPURegisters()
            {
                Control = 0b00100000
            };

            registers.GetSpriteSize().Should().Be(SpriteSize._8x16);
        }

        [Fact]
        public void ExecuteNmiFlag0()
        {
            var registers = new PPURegisters()
            {
                Control = 0
            };

            registers.GetEnableNMI().Should().BeFalse();
        }

        [Fact]
        public void ExecuteNmiFlag1()
        {
            var registers = new PPURegisters()
            {
                Control = 0x80
            };

            registers.GetEnableNMI().Should().BeTrue();
        }

        [Fact]
        public void GrayScaleFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.IsGrayScale().Should().BeFalse();
        }

        [Fact]
        public void GrayScaleTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 1
            };

            registers.IsGrayScale().Should().BeTrue();
        }

        [Fact]
        public void ShowBackgroundInLeftColumnFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.ShowBackgroundInLeftColumn().Should().BeFalse();
        }

        [Fact]
        public void ShowBackgroundInLeftColumnTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 2
            };

            registers.ShowBackgroundInLeftColumn().Should().BeTrue();
        }

        [Fact]
        public void ShowSpritesInLeftColumnFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.ShowSpritesInLeftColumn().Should().BeFalse();
        }

        [Fact]
        public void ShowSpritesInLeftColumnTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 4
            };

            registers.ShowSpritesInLeftColumn().Should().BeTrue();
        }

        [Fact]
        public void ShowBackgroundFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.ShowBackground().Should().BeFalse();
        }

        [Fact]
        public void ShowBackgroundTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 8
            };

            registers.ShowBackground().Should().BeTrue();
        }

        [Fact]
        public void ShowSpritesFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.ShowSprites().Should().BeFalse();
        }

        [Fact]
        public void ShowSpritesTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 16
            };

            registers.ShowSprites().Should().BeTrue();
        }

        [Fact]
        public void EmphasizeRedFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.EmphasizeRed().Should().BeFalse();
        }

        [Fact]
        public void EmphasizeRedTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 32
            };

            registers.EmphasizeRed().Should().BeTrue();
        }

        [Fact]
        public void EmphasizeGreenFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.EmphasizeGreen().Should().BeFalse();
        }

        [Fact]
        public void EmphasizeGreenTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 64
            };

            registers.EmphasizeGreen().Should().BeTrue();
        }

        [Fact]
        public void EmphasizeBlueFalse()
        {
            var registers = new PPURegisters()
            {
                Mask = 0
            };

            registers.EmphasizeBlue().Should().BeFalse();
        }

        [Fact]
        public void EmphasizeBlueTrue()
        {
            var registers = new PPURegisters()
            {
                Mask = 128
            };

            registers.EmphasizeBlue().Should().BeTrue();
        }
    }
}
