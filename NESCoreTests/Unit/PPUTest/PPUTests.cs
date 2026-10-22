using System.Reflection;
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
            var registers = new PPURegisters
            {
                Control = 0x80 //Enable nmi
            };
            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            uint beforeVBlank =
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

            uint beforeVBlank =
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
            var registers = new PPURegisters
            {
                Control = 0x80 //Enable nmi
            };
            var ppu = new PPU(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            for (int i = 0; i < 3; i++) // Three times a charm!
            {
                uint frameCycles = 341 * 261; // Visible screen
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
            uint beforePrerender =
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
            uint beforePrerender =
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
            uint beforePrerender =
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
        public void WriteToScrollSetsTheValue()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();

            var ppu = new PPU(registers.Object, _ppuMemory.Object, _logger);
            
            ppu.Write(0x2005, 0xde);
            ppu.Write(0x2005, 0xad);

            registers.Object.XScroll.Should().Be(0xde);
            registers.Object.YScroll.Should().Be(0xad);
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

        [Fact]
        public void WriteToVRAMIncrementsAddressByOne()
        {
            var ppuMemory = new Mock<IPPUMemory>();

            var registers = new PPURegisters();
            var ppu = new PPU(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            ppu.Write(0x2007, 0xaa);
            ppu.Write(0x2007, 0xab);
            ppu.Write(0x2007, 0xac);

            ppuMemory.Verify(m => m.Write(0x3c0f, 0xaa), Times.Once());
            ppuMemory.Verify(m => m.Write(0x3c10, 0xab), Times.Once());
            ppuMemory.Verify(m => m.Write(0x3c11, 0xac), Times.Once());
        }

        [Fact]
        public void WriteToVRAMIncrementsAddressBy32()
        {
            var ppuMemory = new Mock<IPPUMemory>();

            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.VRAMIncrement()).Returns(32);

            var ppu = new PPU(registers.Object, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            ppu.Write(0x2007, 0xaa);
            ppu.Write(0x2007, 0xab);
            ppu.Write(0x2007, 0xac);

            ppuMemory.Verify(m => m.Write(0x3c0f, 0xaa), Times.Once());
            ppuMemory.Verify(m => m.Write(0x3c2f, 0xab), Times.Once());
            ppuMemory.Verify(m => m.Write(0x3c4f, 0xac), Times.Once());
        }

        [Fact]
        public void ReadFromVRAM()
        {
            var ppuMemory = new Mock<IPPUMemory>();
            ppuMemory.Setup(m => m.Read(0x3c0f)).Returns(0xaa);

            var registers = new PPURegisters();
            var ppu = new PPU(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            var result = ppu.Read(0x2007); //First result is the previously buffered data
            result = ppu.Read(0x2007); // real data byte

            result.Should().Be(0xaa);
            ppuMemory.Verify(m => m.Read(0x3c0f), Times.Once());
        }

        [Fact]
        public void ReadFromVRAMPaletteIsNotBuffered()
        {
            var ppuMemory = new Mock<IPPUMemory>();
            ppuMemory.Setup(m => m.Read(0x3f00)).Returns(0xaa);

            var registers = new PPURegisters();
            var ppu = new PPU(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3f); // Upper byte first
            ppu.Write(0x2006, 0x00); // Lower byte second

            var result = ppu.Read(0x2007); // Palette access is not buffered

            result.Should().Be(0xaa);
            ppuMemory.Verify(m => m.Read(0x3f00), Times.Once());
        }

        [Fact]
        public void ReadFromVRAMIncrementsAddressByOne()
        {
            var ppuMemory = new Mock<IPPUMemory>();
            ppuMemory.Setup(m => m.Read(0x3c0e)).Returns(0x00);
            ppuMemory.Setup(m => m.Read(0x3c0f)).Returns(0xaa);
            ppuMemory.Setup(m => m.Read(0x3c10)).Returns(0xab);
            ppuMemory.Setup(m => m.Read(0x3c11)).Returns(0xac);

            var registers = new PPURegisters();
            var ppu = new PPU(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            var result = ppu.Read(0x2007); //First read should be discared because of the buffering

            result = ppu.Read(0x2007);
            result.Should().Be(0xaa);

            result = ppu.Read(0x2007);
            result.Should().Be(0xab);

            result = ppu.Read(0x2007);
            result.Should().Be(0xac);

            ppuMemory.Verify(m => m.Read(0x3c0f), Times.Once());
            ppuMemory.Verify(m => m.Read(0x3c10), Times.Once());
            ppuMemory.Verify(m => m.Read(0x3c11), Times.Once());
        }

        [Fact]
        public void ReadFromVRAMIncrementsAddressBy32()
        {
            var ppuMemory = new Mock<IPPUMemory>();
            ppuMemory.Setup(m => m.Read(0x3c0e)).Returns(0x00);
            ppuMemory.Setup(m => m.Read(0x3c0f)).Returns(0xaa);
            ppuMemory.Setup(m => m.Read(0x3c2f)).Returns(0xab);
            ppuMemory.Setup(m => m.Read(0x3c4f)).Returns(0xac);

            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.VRAMIncrement()).Returns(32);

            var ppu = new PPU(registers.Object, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            var result = ppu.Read(0x2007); //First read should be discared because of the buffering

            result = ppu.Read(0x2007);
            result.Should().Be(0xaa);

            result = ppu.Read(0x2007);
            result.Should().Be(0xab);

            result = ppu.Read(0x2007);
            result.Should().Be(0xac);

            ppuMemory.Verify(m => m.Read(0x3c0f), Times.Once());
            ppuMemory.Verify(m => m.Read(0x3c2f), Times.Once());
            ppuMemory.Verify(m => m.Read(0x3c4f), Times.Once());
        }

        [Fact]
        public void ReadFromOAMAddressLogs()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();

            var logger = new Mock<ILogger>();

            var ppu = new PPU(registers.Object, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2003);
            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void WriteToOAMAddress()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();


            var ppu = new PPU(registers.Object, _ppuMemory.Object, _logger);
            ppu.Write(0x2003, 0xaa);
            registers.Object.OAMAddress.Should().Be(0xaa);
        }

        [Fact]
        public void WriteToOAMDataWritesValueToOamAndIncrementsOAMAddress()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Object.OAMAddress = 0x0;

            var memory = new Mock<IPPUMemory>();

            var ppu = new PPU(registers.Object, memory.Object, _logger);
            ppu.Write(0x2004, 0xde);

            memory.Verify(m => m.OamWrite(0x0, 0xde));
            registers.Object.OAMAddress.Should().Be(1);
        }

        [Fact]
        public void ReadFromOAMDataReadsValueFromOamAndDoesNotIncrementOAMAddress()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Object.OAMAddress = 0x0;

            var memory = new Mock<IPPUMemory>();
            memory.Setup(m => m.OamRead(0)).Returns(0xde);

            var ppu = new PPU(registers.Object, memory.Object, _logger);
            var value = ppu.Read(0x2004);
            value.Should().Be(0xde);

            memory.Verify(m => m.OamRead(0x0));
            registers.Object.OAMAddress.Should().Be(0);
        }

        [Fact]
        public void ShouldFetchNameTableOnVisibleCycle2()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            // V = 2042
            ppu.Write(0x2006, 0x20);
            ppu.Write(0x2006, 0x42);
            memory.Setup(m => m.Read(0x2042)).Returns(0x77);
            
            ppu.Run(2);
            
            memory.Verify(m => m.Read(0x2042), Times.Once);
        }
        
        [Fact]
        public void ShouldFetchAttributeTableOnVisibleCycle4()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            ppu.Write(0x2006, 0x00);
            ppu.Write(0x2006, 0x8C);
            // NT read
            memory.Setup(m => m.Read(0x208C)).Returns(0x00);
            // AT read
            memory.Setup(m => m.Read(0x23CB)).Returns(0xAA);

            ppu.Run(4);

            memory.Verify(m => m.Read(0x23CB), Times.Once);
            
            var field = typeof(PPU).GetField("_attributeId", BindingFlags.NonPublic | BindingFlags.Instance);
            var attributeId = field?.GetValue(ppu);
            attributeId.Should().Be(0xAA);
        }

        [Fact]
        public void ShouldFetchPatternTableLowCycle6()
        {
            var registers = new PPURegisters
            {
                Control = 0x10 // BG pattern table in the second thebe (0x1000-0x1fff)
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            ppu.Write(0x2006, 0x00);
            ppu.Write(0x2006, 0x8C);
            // NT read
            memory.Setup(m => m.Read(0x208C)).Returns(0x41); // Tile ID
            // AT read
            memory.Setup(m => m.Read(0x23CB)).Returns(0xAA);
            // PT low read
            memory.Setup(m => m.Read(0x1410)).Returns(0b01010101);
            
            ppu.Run(6);
            
            // 0x1000 + 0x41 (65) * 16 (+ fineY) => 0x1410
            memory.Verify(m => m.Read(0x1410), Times.Once);
        }
        
        [Fact]
        public void ShouldFetchPatternTableHighCycle8()
        {
            var registers = new PPURegisters
            {
                Control = 0x10 // BG pattern table in the second thebe (0x1000-0x1fff)
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            ppu.Write(0x2006, 0x00);
            ppu.Write(0x2006, 0x8C);
            // NT read
            memory.Setup(m => m.Read(0x208C)).Returns(0x41); // Tile ID
            // AT read
            memory.Setup(m => m.Read(0x23CB)).Returns(0xAA);
            // PT low read
            memory.Setup(m => m.Read(0x1410)).Returns(0b01010101);
            // PT high read
            memory.Setup(m => m.Read(0x1418)).Returns(0b11001100);
            
            ppu.Run(8);
            
            // 0x1000 + 0x41 (65) * 16 (+ fineY) + 8 => 0x1418
            memory.Verify(m => m.Read(0x1418), Times.Once);
        }

        [Fact]
        public void ShouldIncrementCoarseXOnCycle8()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            // set ppu address to 0x2000 => coarseX = 0
            ppu.Write(0x2006, 0x20);
            ppu.Write(0x2006, 0x00);
            memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0x00);

            ppu.Run(8);
            
            var field = typeof(PPU).GetField("_ppuAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var ppuAddress = field?.GetValue(ppu);
            ppuAddress.Should().Be(0x2001);
        }

        [Fact]
        public void CoarseXShouldWrapAroundOn32()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            // set coarseX to 32
            ppu.Write(0x2006, 0x00);
            ppu.Write(0x2006, 0x1F);
            memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0);
            
            ppu.Run(8);
            
            var field = typeof(PPU).GetField("_ppuAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var ppuAddress = field?.GetValue(ppu);
            ppuAddress.Should().Be(0x0400);
        }

        [Fact]
        public void ShouldIncrementFineYOnCycle256()
        {
            // We are at the end of the scanline, so we should wrap around to the next tile row (increment fineY)
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            ppu.Write(0x2006, 0x20);
            ppu.Write(0x2006, 0x00);
            memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0);
            
            ppu.Run(256);
            
            var field = typeof(PPU).GetField("_ppuAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var ppuAddress = field?.GetValue(ppu);
            // Fine Y should be incremented by 1: 0 001 00 00000 00000 + 0x2000 + (0x400 because we also increased X) = 0x3400
            ppuAddress.Should().Be(0x3400); // We are in the same tile row but down one scanline
        }

        [Fact]
        public void ShouldRestoreXAtTheEndOfTheScanline()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            ppu.Write(0x2006, 0x20);
            ppu.Write(0x2006, 0x00);
            memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0);
            
            ppu.Run(257);
            
            var field = typeof(PPU).GetField("_ppuAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var ppuX = field?.GetValue(ppu);
            ppuX.Should().Be(0x3000); // We are back to the start of the tile row
        }
        
        [Fact]
        public void ShouldWrapFineYAndIncrementCoarseY_WhenFineYIs7()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            // V = 0x3000 => fineY = 3
            ppu.Write(0x2006, 0x30);
            ppu.Write(0x2006, 0x00);
            memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0);
            
            // run until coarseY increments
            
            // 4 full scanlines => + 5th sc 256 cycle
            ppu.Run(1620);
            
            var field = typeof(PPU).GetField("_ppuAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var ppuAddress = field?.GetValue(ppu);
            ppuAddress.Should().Be(0x0420);
        }

        [Fact]
        public void ShouldIncrementNameTableBit()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            // fine Y = 3 => 0x3000
            // coarse Y = 29 => 29 << 5 => 928 => 0x03A0
            ppu.Write(0x2006, 0x33);
            ppu.Write(0x2006, 0xA0);
            
            // run for 4 full scanlines
            ppu.Run(1620);
            
            var field = typeof(PPU).GetField("_ppuAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var ppuAddress = field?.GetValue(ppu);
            // fine Y = 0
            // coarse Y = 0
            // NT horizontal = 1 0x800
            // Coarse X = 0x400
            ppuAddress.Should().Be(0x0C00);
        }

        [Fact]
        public void WriteToScrollForTheFirstTime_SetsCoarseScrollInTheTempAddressRegister()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            ppu.Write(0x2005, 0x7D);

            var field = typeof(PPU).GetField("_tempPPUAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var tempAddress = (ushort)field!.GetValue(ppu);
            var latchField = typeof(PPU).GetField("_addressLatch", BindingFlags.NonPublic | BindingFlags.Instance);
            var latch = (bool)latchField!.GetValue(ppu);
            
            // temp low 5 bits is coarse X
            var tempCoarseX = tempAddress & 0x1F;
            tempCoarseX.Should().Be(0xF);
            registers.CoarseX.Should().Be(0xF); // also set in the background registers
            registers.FineX.Should().Be(0x05);
            latch.Should().BeTrue();
        }
        
        [Fact]
        public void WriteToScrollForTheSecondTime_SetsYScrollInTheTempAddressRegister()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            ppu.Write(0x2005, 0x7D);
            ppu.Write(0x2005, 0b10101010); // second write is tested here

            var field = typeof(PPU).GetField("_tempPPUAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var tempAddress = (ushort)field!.GetValue(ppu);
            var latchField = typeof(PPU).GetField("_addressLatch", BindingFlags.NonPublic | BindingFlags.Instance);
            var latch = (bool)latchField!.GetValue(ppu);
            
            //FGH..AB CDE..... <- d: ABCDEFGH
            
            //: 0 yyy NN YYYYY XXXXX 
            var tempFineY = (tempAddress & 0x7000) >> 12;
            tempFineY.Should().Be(0b010);
            registers.FineY.Should().Be(0b010);
            
            var tempCoarseY = (tempAddress & 0x3e0) >> 5;
            tempCoarseY.Should().Be(0b10101);
            registers.CoarseY.Should().Be(0b10101);
            latch.Should().BeFalse();
        }
    }
}
