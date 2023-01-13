using NESCore.PPU;

namespace NESCoreTests.Unit.PPUTest
{
    public class PPUTests : TestBase
    {
        [Fact]
        public void ReadFromStatusOnlyReturnsTheTop3bits()  //TODO: openbus behaviour. Return previous data from the ppu databuffer on the lower bits
        {
            byte testData = 0xff;
            var ppuRegisters = new PPURegisters
            {
                Status = testData
            };

            var ppu = new PPU(ppuRegisters, _logger);
            var data = ppu.Read(0x2002);
            data.Should().Be(0b11100000);
        }

        [Fact]
        public void ReadFromStatusResetsVBlankFlag()
        {
            byte testData = 0xE0;
            var ppuRegisters = new PPURegisters
            {
                Status = testData
            };
            var ppu = new PPU(ppuRegisters, _logger);
            var status = ppu.Read(0x2002);
            status.Should().Be(0b11100000);
            ppuRegisters.Status.Should().Be(0b01100000);
        }

        [Fact]
        public void SetsVBlankFlag()
        {
            var registers = new PPURegisters();
            var ppu = new PPU(registers, _logger);
            var beforeVBlank =
                341 * 240 //Visible screen
                + 341; //post render scanline

            ppu.Run(beforeVBlank);
            registers.GetVBlankFlag().Should().BeFalse();
            ppu.Run(2); //set the vblank flag on the first cycle of the vblank lines

            registers.GetVBlankFlag().Should().BeTrue();
        }

        [Fact]
        public void ClearsVBlankFlag()
        {
            var registers = new PPURegisters();
            var ppu = new PPU(registers, _logger);
            var beforePrerender =
                341 * 240
                + 341
                + 2;
            ppu.Run(beforePrerender);
            registers.GetVBlankFlag().Should().BeTrue(); //Double check if vblank is set

            ppu.Run(20 * 341);

            registers.GetVBlankFlag().Should().BeFalse();
        }

        [Fact]
        public void ClearsOverflowFlag()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            var ppu = new PPU(registers, _logger);
            var beforePrerender =
                341 * 240
                + 341
                + 2;
            ppu.Run(beforePrerender);
            ppu.Run(20 * 341);

            registers.GetOverflowFlag().Should().BeFalse();
        }

        [Fact]
        public void ClearsSpriteZeroHitFlag()
        {
            var registers = new PPURegisters
            {
                Status = 0xff
            };

            var ppu = new PPU(registers, _logger);
            var beforePrerender =
                341 * 240
                + 341
                + 2;
            ppu.Run(beforePrerender);
            ppu.Run(20 * 341);

            registers.GetSpriteZeroHitFlag().Should().BeFalse();
        }
    }
}
