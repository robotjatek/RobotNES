using Moq;

using NESCore.PPU;

using Serilog;

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

            var ppu = new PPU(ppuRegisters, _ppuMemory.Object, _logger);
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
            var ppu = new PPU(ppuRegisters, _ppuMemory.Object, _logger);
            var status = ppu.Read(0x2002);
            status.Should().Be(0b11100000);
            ppuRegisters.Status.Should().Be(0b01100000);
        }

        [Fact]
        public void SetsVBlankFlagAndSendsNMI()
        {
            var registers = new PPURegisters();
            registers.Control = 0x80; //Enable nmi
            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            var beforeVBlank =
                341 * 240 // Visible screen
                + 341; // Post render scanline

            ppu.Run(beforeVBlank);
            registers.GetVBlankFlag().Should().BeFalse();
            ppu.Run(2); // set the vblank flag on the first cycle of the vblank lines

            registers.GetVBlankFlag().Should().BeTrue();
            eventMonitor.Should().Raise(nameof(ppu.NMIEvent));
        }

        [Fact]
        public void SetsVBlankFlagButDoesNotSendNMI()
        {
            var registers = new PPURegisters();
            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            var beforeVBlank =
                341 * 240 // Visible screen
                + 341; // Post render scanline

            ppu.Run(beforeVBlank);
            registers.GetVBlankFlag().Should().BeFalse();
            ppu.Run(2); // set the vblank flag on the first cycle of the vblank lines

            registers.GetVBlankFlag().Should().BeTrue();
            eventMonitor.Should().NotRaise(nameof(ppu.NMIEvent));
        }

        [Fact]
        public void SetsVBlankFlagAndSendsNMIMultipleTimes()
        {
            var registers = new PPURegisters();
            registers.Control = 0x80; //Enable nmi
            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            for (int i = 0; i < 3; i++) // Three times a charm!
            {
                var frameCycles = 341 * 261; // Visible screen
                ppu.Run(frameCycles);

                registers.GetVBlankFlag().Should().BeTrue();
            }

            eventMonitor.OccurredEvents.Should().HaveCount(3);
        }

        [Fact]
        public void ClearsVBlankFlag()
        {
            var registers = new PPURegisters();
            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            var beforePrerender =
                341 * 240
                + 341
                + 2;
            ppu.Run(beforePrerender);
            registers.GetVBlankFlag().Should().BeTrue(); // Double check if vblank is set

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

            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
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

            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            var beforePrerender =
                341 * 240
                + 341
                + 2;
            ppu.Run(beforePrerender);
            ppu.Run(20 * 341);

            registers.GetSpriteZeroHitFlag().Should().BeFalse();
        }

        [Fact]
        public void WriteToControlRegister()
        {
            var registers = new PPURegisters
            {
                Control = 0
            };

            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            ppu.Write(0x2000, 0xff);

            registers.Control.Should().Be(0xff);
        }

        [Fact]
        public void ReadFromControlRegisterLogs()
        {
            var registers = new PPURegisters();
            var logger = new Mock<ILogger>();
            var ppu = new PPU(registers, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2000);

            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void WriteToMaskRegister()
        {
            var registers = new PPURegisters
            {
                Mask = 0
            };
            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            ppu.Write(0x2001, 0xff);

            registers.Mask.Should().Be(0xff);
        }

        [Fact]
        public void ReadFromMaskRegisterLogs()
        {
            var registers = new PPURegisters();
            var logger = new Mock<ILogger>();
            var ppu = new PPU(registers, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2001);

            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void ReadFromScrollLogs()
        {
            var logger = new Mock<ILogger>();
            var ppu = new PPU(_ppuRegisters.Object, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2005);

            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void ReadFromAddressLogs()
        {
            var logger = new Mock<ILogger>();
            var ppu = new PPU(_ppuRegisters.Object, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2006);

            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void WriteToVRAM()
        {
            var ppuMemory = new Mock<IPPUMemory>();

            var registers = new PPURegisters();
            var ppu = new PPU(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second
            ppu.Write(0x2007, 0xaa); // Data byte

            ppuMemory.Verify(m => m.Write(0x3c0f, 0xaa), Times.Once());
        }

        // TODO: address latch reset test
    }
}
