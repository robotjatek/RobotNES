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

            ppu.Read(0x2007); //First result is the previously buffered data
            var result = ppu.Read(0x2007); // real data byte

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
            var registers = new PPURegisters { Mask = 0x8 };
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
            var registers = new PPURegisters { Mask = 0x8 };
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
                Control = 0x10, // BG pattern table in the second table (0x1000-0x1fff)
                Mask = 0x8
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
                Control = 0x10, // BG pattern table in the second table (0x1000-0x1fff)
                Mask = 0x8
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
            var registers = new PPURegisters { Mask = 0x8 };
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
            var registers = new PPURegisters() { Mask = 0x8 }; // Show background
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
            var registers = new PPURegisters { Mask = 0x8 };
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
            var registers = new PPURegisters { Mask = 0x8 };
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
            var registers = new PPURegisters { Mask = 0x8 };
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
            ppuAddress.Should().Be(0x0422);
        }

        [Fact]
        public void ShouldIncrementNameTableBit()
        {
            var registers = new PPURegisters { Mask = 0x8 };
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
            // + 2 prefetch reads
            ppuAddress.Should().Be(0x0C02);
        }

        [Fact]
        public void WriteToScrollForTheFirstTime_SetsCoarseScrollInTheTempAddressRegister()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            ppu.Write(0x2005, 0x7D);

            var field = typeof(PPU).GetField("_tempAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var tempAddress = (ushort)field!.GetValue(ppu)!;
            var latchField = typeof(PPU).GetField("_addressLatch", BindingFlags.NonPublic | BindingFlags.Instance);
            var latch = (bool)latchField!.GetValue(ppu)!;
            
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

            var field = typeof(PPU).GetField("_tempAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var tempAddress = (ushort)field!.GetValue(ppu)!;
            var latchField = typeof(PPU).GetField("_addressLatch", BindingFlags.NonPublic | BindingFlags.Instance);
            var latch = (bool)latchField!.GetValue(ppu)!;
            
            //FGH..AB CDE..... <- d: ABCDEFGH
            
            //t: 0 yyy NN YYYYY XXXXX 
            var tempFineY = (tempAddress & 0x7000) >> 12;
            tempFineY.Should().Be(0b010);
            registers.FineY.Should().Be(0b010);
            
            var tempCoarseY = (tempAddress & 0x3e0) >> 5;
            tempCoarseY.Should().Be(0b10101);
            registers.CoarseY.Should().Be(0b10101);
            latch.Should().BeFalse();
        }

        [Fact]
        public void ControlRegisterWrite_ShouldUpdateNameTableBitsInTemp()
        {
            var registers = new PPURegisters();
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            ppu.Write(0x2000, 0b00000011);
            var field = typeof(PPU).GetField("_tempAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var tempAddress = (ushort)field!.GetValue(ppu)!;
            
            var nt = (tempAddress & 0x0C00) >> 10;
            //t: 0 yyy NN YYYYY XXXXX
            nt.Should().Be(0b11);
        }

        [Fact]
        public void StatusRegisterRead_ShouldResetTheLatch()
        {
            var ppu = new PPU(_ppuRegisters.Object, _ppuMemory.Object, _logger);
            
            ppu.Write(0x2005, 1);
            
            var latchField = typeof(PPU).GetField("_addressLatch", BindingFlags.NonPublic | BindingFlags.Instance);
            var latch = (bool)latchField!.GetValue(ppu)!;
            latch.Should().BeTrue();

            ppu.Read(0x2002);
            
            latchField = typeof(PPU).GetField("_addressLatch", BindingFlags.NonPublic | BindingFlags.Instance);
            latch = (bool)latchField!.GetValue(ppu)!;
            latch.Should().BeFalse();
        }

        [Fact]
        public void ShouldReloadShiftRegisters()
        {
            // Shift registers are used to output pixels.
            // There are 2 shift registers, one for the low bits and one for the high bits from the pattern table.
            // These bytes were loaded from the pattern table in the previous cycles.
            var registers = new PPURegisters
            {
                Control = 0x10, // BG base 0x1000
                Mask = 0x8
            };
            
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            ppu.Write(0x2006, 0);
            ppu.Write(0x2006, 0x8C);
            
            memory.Setup(m => m.Read(0x208C)).Returns(0x41);
            memory.Setup(m => m.Read(0x22CB)).Returns(0x00);
            memory.Setup(m => m.Read(0x1410)).Returns(0b11110000); // pattern-lo
            memory.Setup(m => m.Read(0x1418)).Returns(0b10101010); // pattern-hi
            
            ppu.Run(8);
            
            memory.Verify(m => m.Read(0x208C));
            
            var shiftLo = typeof(PPU).GetField("_shiftRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance);
            var shiftHi = typeof(PPU).GetField("_shiftRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var lowValue = (ushort)shiftLo!.GetValue(ppu)!;
            var highValue = (ushort)shiftHi!.GetValue(ppu)!;
            
            (lowValue & 0x00FF).Should().Be(0b11110000);
            (highValue & 0x00FF).Should().Be(0b10101010);
        }

        [Fact]
        public void ShiftRegisters_ShouldShiftLeftOneBit_DuringVisibleCycles()
        {
            // same as the previous test, only shift registers should be shifted left one bit
            var registers = new PPURegisters()
            {
                Control = 0x10,
                Mask = 0x8
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            ppu.Write(0x2006, 0);
            ppu.Write(0x2006, 0x8C);

            memory.Setup(m => m.Read(0x208c)).Returns(0x41);
            memory.Setup(m => m.Read(0x22CB)).Returns(0);
            memory.Setup(m => m.Read(0x1410)).Returns(0b11110000);
            memory.Setup(m => m.Read(0x1418)).Returns(0b10101010);
            
            ppu.Run(8); // Load the shift registers with the first 8 bits
            ppu.Run(1); // Shift left one bit
            
            var shiftLo = typeof(PPU).GetField("_shiftRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance);
            var shiftHi = typeof(PPU).GetField("_shiftRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance);

            var lowValue = (ushort)shiftLo!.GetValue(ppu)!;
            var highValue = (ushort)shiftHi!.GetValue(ppu)!;

            // registers should be shifted left one bit
            lowValue.Should().Be(0b111100000);
            highValue.Should().Be(0b101010100);
        }

        [Fact]
        public void ShouldReloadAttributeShiftRegisterWithCorrect2BitsOnCycle8()
        {
            // every pixel needs two bits from AT
            // AT bits:
            //  0,1 top-left
            //  2,3 top-right
            //  4-5 bottom-left
            //  6-7 bottom right
            
            // coarse X and coarse y determine which quarter of the AT to use
            // coarse X bit 1 == 1 => right
            // coarse Y bit 1 == 1 => bottom
            
            var registers = new PPURegisters
            {
                Control = 0x10,
                Mask = 0x8
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            // coarse X = 2 (bit 1 = 1), coarse y = 2 (bit 1 = 1) [right bottom at bits]
            ppu.Write(0x2006, 0);
            ppu.Write(0x2006, 0x42);
            
            memory.Setup(m => m.Read(0x2042)).Returns(0);
            // $23C0 | 0 | 0 | 0 = $23C0
            memory.Setup(m => m.Read(0x23C0)).Returns(0b11000000); // bottom right palette is set to 3

            ppu.Run(8);
            
            var attrLo = typeof(PPU).GetField("_attributeRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance);
            var attrHi = typeof(PPU).GetField("_attributeRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var attrLoValue = (ushort)attrLo!.GetValue(ppu)!;
            var attrHiValue = (ushort)attrHi!.GetValue(ppu)!;

            (attrLoValue & 0x01).Should().Be(1);
            (attrHiValue & 0x01).Should().Be(1);
        }

        [Fact]
        public void ShouldCopyVerticalBitsFromTempAddress()
        {
            var registers = new PPURegisters
            {
                Control = 0x10,
                Mask = 0x8
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            ppu.Write(0x2005, 0); // X scroll 0
            ppu.Write(0x2005, 0x5e); // y scroll => fineY = 6, CoarseY = 11: temp address = $6160
            ppu.Run(261 * 341 + 304); // scanline 261, cycle 304
            
            var field = typeof(PPU).GetField("_ppuAddress", BindingFlags.NonPublic | BindingFlags.Instance);
            var address = (ushort)field!.GetValue(ppu)!;
            ((address & 0x7000) >> 12).Should().Be(6); // fine y
            ((address & 0x03e0) >> 5).Should().Be(11); // coarse Y
        }

        [Fact]
        public void ShouldKeepFetchingOnNonVisibleCycles()
        {
            // ppu does not stop fetching during non-visible cycles
            // NT, AT, PATTERN1, PATTERN2 fetches keep happening after between cycle 320 and 340 to prefetch the next 2 tiles for the next scanline

            var registers = new PPURegisters { Control = 0x10, Mask = 0x8 };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            ppu.Write(0x2006, 0x20);
            ppu.Write(0x2006, 0x0);
            memory.Setup(m => m.Read(0x2000)).Returns(0x99);
            memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0);
            
            ppu.Run(321); // the first pre-fetch nt read should happen here
            
            memory.Verify(m => m.Read(0x2000), Times.Once);
        }

        [Fact]
        public void ShouldGetCurrentPixelPaletteAddressFromShiftRegisters()
        {
            var registers = new PPURegisters { Control = 0x10 };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            typeof(PPU).GetField("_shiftRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000); // bit 0 = 1
            typeof(PPU).GetField("_shiftRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000); // bit 1 = 1 -> Color Index = 3
            typeof(PPU).GetField("_attributeRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000);  // bit 0 = 1
            typeof(PPU).GetField("_attributeRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000);  // bit 1 = 1 -> Palette ID = 3
            
            var paletteAddress = ppu.GetCurrentPixelPaletteAddressTest();
            // 0x3F00 | (3 << 2) | 3 = 0x3F0F
            paletteAddress.Should().Be(0x3F0F);
        }

        [Fact]
        public void OnVisibleScanline_ShouldRenderPixelFromPalette()
        {
            var registers = new PPURegisters { Mask = 0x8 };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            typeof(PPU).GetField("_shiftRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000);
            typeof(PPU).GetField("_shiftRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000);
            typeof(PPU).GetField("_attributeRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000);
            typeof(PPU).GetField("_attributeRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ppu, (ushort)0x8000);
            
            memory.Setup(m => m.Read(0x3F0F)).Returns(0x15);
            
            ppu.Run(1);
            
            var bufferField = typeof(PPU).GetField("_frameBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var buffer = (byte[])bufferField!.GetValue(ppu)!;
          
            // NTSC colors
            buffer[2].Should().Be(228); // R
            buffer[1].Should().Be(0); // G
            buffer[0].Should().Be(88); // B
        }

        [Fact]
        public void ShouldSetVBlankFlagAtTheExactStartOfVBlank()
        {
            var registers = new PPURegisters { Control = 0x80 }; // nmi enabled: bit 7 = 1
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            using var monitor = ppu.Monitor();

            uint cyclesBeforeVBlank = 240 * 341 + 341;
            ppu.Run(cyclesBeforeVBlank);
            registers.GetVBlankFlag().Should().BeFalse();

            ppu.Run(1);

            registers.GetVBlankFlag().Should().BeTrue();
            monitor.Should().Raise(nameof(ppu.NMIEvent));
        }

        [Fact]
        public void Should_HaveEvaluatedSpritesInSecondaryOamOnTheSecondScanline()
        {
            var registers = new PPURegisters
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprite pattern table address: 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => i == 0 ? 
                    new OamEntry(0, 0, 0, 10, 0) :
                    new OamEntry(0xEF, 0, 0, 10, 0)); // initialize oam with sprites outside the visible area

            ppu.Run(341 * 2); // end of the 2nd scanline
            
            memory.Verify(m => m.GetOamEntry(0), Times.Exactly(2));
            
            var secondaryOamField = typeof(PPU).GetField("_secondaryOam", BindingFlags.NonPublic | BindingFlags.Instance);
            var secondaryOam = (List<OamEntry>)secondaryOamField!.GetValue(ppu)!;
            secondaryOam.Count.Should().Be(1);
            secondaryOam[0].Should().BeEquivalentTo(new OamEntry(0, 0, 0, 10, 0));
        }
        
        [Fact]
        public void Should_HaveTwoEvaluatedSpritesInSecondaryOAM()
        {
            var registers = new PPURegisters
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprite pattern table address: 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    return i switch
                    {
                        0 => new OamEntry(49, 0, 0, 10, 0), // Starts in 50th row
                        1 => new OamEntry(45, 0, 0, 10, 1), // Starts in 46th row, it is in the scanline
                        _ => new OamEntry(0xEF, 0, 0, 10, i) // Not evaluated
                    };
                });

            ppu.Run(341 * 50 + 1);
            
            var secondaryOamField = typeof(PPU).GetField("_secondaryOam", BindingFlags.NonPublic | BindingFlags.Instance);
            var secondaryOam = (List<OamEntry>)secondaryOamField!.GetValue(ppu)!;
            secondaryOam.Count.Should().Be(2);
            secondaryOam[0].Should().BeEquivalentTo(new OamEntry(49, 0, 0, 10, 0));
            secondaryOam[1].Should().BeEquivalentTo(new OamEntry(45, 0, 0, 10, 1));
        }

        [Fact]
        public void ShouldNot_HaveSpritesOnTheFirstScanline()
        {
            var registers = new PPURegisters
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprite pattern table address: 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    return i switch
                    {
                        0 => new OamEntry(0, 0, 0, 10, 0),
                        1 => new OamEntry(0, 0, 0, 10, 1),
                        _ => new OamEntry(0xEF, 0, 0, 10, i) // Not evaluated
                    };
                });

            ppu.Run(341 * 50 + 1);
            
            var secondaryOamField = typeof(PPU).GetField("_secondaryOam", BindingFlags.NonPublic | BindingFlags.Instance);
            var secondaryOam = (List<OamEntry>)secondaryOamField!.GetValue(ppu)!;
            secondaryOam.Count.Should().Be(0);
        }
        
        [Fact]
        public void Should_OnlyHave8SpritesPerScanline()
        {
            var registers = new PPURegisters
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprite pattern table address: 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    return i switch
                    {
                        >=0 and <10 => new OamEntry(0, 0, 0, 10, 0), // Return 10 sprites in the scanline
                        _ => new OamEntry(0xEF, 0, 0, 10, i) // Not evaluated
                    };
                });

            ppu.Run(341 * 2);
            
            var secondaryOamField = typeof(PPU).GetField("_secondaryOam", BindingFlags.NonPublic | BindingFlags.Instance);
            var secondaryOam = (List<OamEntry>)secondaryOamField!.GetValue(ppu)!;
            secondaryOam.Count.Should().Be(8);
            registers.GetOverflowFlag().Should().BeTrue();
        }

        [Fact]
        public void SpriteOverflowDoesNotResetOnTheNextScanline()
        {
            // same as the Should_OnlyHave8SpritesPerScanline test but runs longer 
            var registers = new PPURegisters
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprite pattern table address: 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    return i switch
                    {
                        >=0 and <10 => new OamEntry(0, 0, 0, 10, 0), // Return 10 sprites in the scanline
                        _ => new OamEntry(0xEF, 0, 0, 10, i) // Not evaluated
                    };
                });

            ppu.Run(341 * 60);
            
            var secondaryOamField = typeof(PPU).GetField("_secondaryOam", BindingFlags.NonPublic | BindingFlags.Instance);
            var secondaryOam = (List<OamEntry>)secondaryOamField!.GetValue(ppu)!;
            secondaryOam.Count.Should().Be(0); // No sprites here
            registers.GetOverflowFlag().Should().BeTrue(); // But the overflow flag is still set
        }

        [Fact]
        public void SpriteOverflowFlagResetOnTheNextScanline()
        {
            // Render a frame with 10 in a scanline
            // Then start a new frame => overflow bit should be reset
            var registers = new PPURegisters
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprite pattern table address: 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    return i switch
                    {
                        >=0 and <10 => new OamEntry(45, 0, 0, 10, 0), // Return 10 sprites in the scanline
                        _ => new OamEntry(0xEF, 0, 0, 10, i) // Not evaluated
                    };
                });

            ppu.Run(341 * 262); // Run the PPU to the next frame
            
            var secondaryOamField = typeof(PPU).GetField("_secondaryOam", BindingFlags.NonPublic | BindingFlags.Instance);
            var secondaryOam = (List<OamEntry>)secondaryOamField!.GetValue(ppu)!;
            secondaryOam.Count.Should().Be(0); // No sprites here
            registers.GetOverflowFlag().Should().BeFalse(); // The overflow flag is reset on the next scanline
        }
        
        [Fact]
        public void ShouldDrawIntoTheSpriteScanlineBuffer()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprites on 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        // X = 0-7, Palette 3 
                        0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(3, true, false, false).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });

            // merged pattern 12....2. with third palette (2) => DE....E. => + 0x3f10  => 0x3F1F, 0x3F1F + 8,   
            const byte pattern1 = 0b10000000;
            const byte pattern2 = 0b01000010;
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // planes
                        0x1000 => pattern1,
                        0x1008 => pattern2,
                        
                        // palette px 1
                        0x3F1D => 10,
                        
                        // palette px 2, 3
                        0x3F1E => 13,
                        
                        _ => 0
                    };
                });
            
            ppu.Run(341 * 2); // the first scanline cannot have sprites
            
            var spriteScanlineBufferField = typeof(PPU).GetField("_spriteScanlineBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var spriteScanlineBuffer = (PPU.Color?[])spriteScanlineBufferField!.GetValue(ppu)!;
            spriteScanlineBuffer.Where(c => c != null).Should().HaveCount(3); // three visible pixels

            var expected = new List<PPU.Color?>();
            for (var i = 0; i < 256; i++)
            {
                expected.Add(null);
            }
            expected[0] = PPU.Palette[10];
            expected[1] = PPU.Palette[13];
            expected[6] = PPU.Palette[13];
            
            spriteScanlineBuffer.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void FirstSpriteWinsOnOverlap()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(1, true, false, false).ToByte(), 0, i), 
                        1 => new OamEntry(0, 1, new OamEntry.SpriteAttribute(3, true, false, false).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });

            const byte pattern1 = 0b00000000;
            const byte pattern2 = 0b11111111;
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // First sprite
                        0x1000 => pattern1,
                        0x1008 => pattern2,
                        
                        // Second sprite
                        0x1010 => pattern1,
                        0x1018 => pattern2,
                        
                        0x3f16 => 3, // PaletteId for sprite1
                        0x3f1E => 4, // PaletteID for sprite2
                        _ => 0
                    };
                });
            
            ppu.Run(341 * 2);
            
            var spriteScanlineBufferField = ppu.GetType().GetField("_spriteScanlineBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var spriteScanlineBuffer = (PPU.Color?[])spriteScanlineBufferField!.GetValue(ppu)!;
            spriteScanlineBuffer.Where(c => c != null).Should().HaveCount(8);

            var expected = new List<PPU.Color?>();
            for (var i = 0; i < 256; i++)
            {
                expected.Add(null);
            }
            
            expected[0] = PPU.Palette[3];
            expected[1] = PPU.Palette[3];
            expected[2] = PPU.Palette[3];
            expected[3] = PPU.Palette[3];
            expected[4] = PPU.Palette[3];
            expected[5] = PPU.Palette[3];
            expected[6] = PPU.Palette[3];
            expected[7] = PPU.Palette[3];
            
            spriteScanlineBuffer.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void ShouldFlipSpriteHorizontally()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(1, true, false, true).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });
            
            const byte pattern1 = 0b00000001;
            const byte pattern2 = 0b11110001;
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // First sprite
                        0x1000 => pattern1,
                        0x1008 => pattern2,
                        
                        0x3f16 => 3, // PaletteId for sprite1
                        _ => 0
                    };
                });
            
            ppu.Run(341 * 2);
            
            var spriteScanlineBufferField = ppu.GetType().GetField("_spriteScanlineBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var spriteScanlineBuffer = (PPU.Color?[])spriteScanlineBufferField!.GetValue(ppu)!;
            spriteScanlineBuffer.Where(c => c != null).Should().HaveCount(5);
            
            var expected = new List<PPU.Color?>();
            for (var i = 0; i < 256; i++)
            {
                expected.Add(null);
            }

            expected[0] = PPU.Palette[0];
            expected[1] = null;
            expected[2] = null;
            expected[3] = null;
            expected[4] = PPU.Palette[3];
            expected[5] = PPU.Palette[3];
            expected[6] = PPU.Palette[3];
            expected[7] = PPU.Palette[3];

            spriteScanlineBuffer.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void ShouldFlipSpriteVertically()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) =>
                {
                    return i switch
                    {
                        0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(1, true, true, false).ToByte(), 0, i),
                        _ => new OamEntry(0xEF, 0, 0, 10, i)
                    };
                });
            
            const byte pattern1 = 0b00000001;
            const byte pattern2 = 0b11110001;
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // last line of the sprite
                        0x1007 => pattern1,
                        0x100F => pattern2,
                        
                        0x3f16 => 3, // PaletteId for sprite1
                        _ => 0
                    };
                });
            
            ppu.Run(341*2);
            
            var spriteScanlineBufferField = ppu.GetType().GetField("_spriteScanlineBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var spriteScanlineBuffer = (PPU.Color?[])spriteScanlineBufferField!.GetValue(ppu)!;
            spriteScanlineBuffer.Where(c => c != null).Should().HaveCount(5);

            var expected = new List<PPU.Color?>();
            for (var i = 0; i < 256; i++)
            {
                expected.Add(null);
            }
            
            expected[0] = PPU.Palette[3];
            expected[1] = PPU.Palette[3];
            expected[2] = PPU.Palette[3];
            expected[3] = PPU.Palette[3];
            expected[4] = null;
            expected[5] = null;
            expected[6] = null;
            expected[7] = PPU.Palette[0];
            
            spriteScanlineBuffer.Should().BeEquivalentTo(expected);
        }
        
        
        // TODO: sprite priority
        [Fact]
        public void ShouldRenderBackgroundInsteadOfSprite()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprites on 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES | PPUMaskFlagPositions.SHOW_BACKGROUND
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new PPU(registers, memory.Object, _logger);
            
            // Sprite setup
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        // X = 0-7, Palette 3 
                        0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(3, BehindBackground: true, false, false).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });
            
            const byte pattern1 = 0b10000000;
            const byte pattern2 = 0b01000010;
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // planes
                        0x1000 => pattern1,
                        0x1008 => pattern2,
                        
                        // palette px 1 (first sprite pixel from palette index 4)
                        0x3F1D => 4,
                        
                        // palette px 2, 3
                        //0x3F1E => 13,
                        
                        _ => 0
                    };
                });
            
            memory.Setup(m => m.Read(0x3F0F)).Returns(21); // Background color from palette index 21
            
            ppu.Run(341); // First line
            
            // set the background bits on the second line into the shift registers
            typeof(PPU).GetField("_shiftRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance)!.
                SetValue(ppu, (ushort)0x8000);
            typeof(PPU).GetField("_shiftRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance)!.
                SetValue(ppu, (ushort)0x8000);
            typeof(PPU).GetField("_attributeRegisterLo", BindingFlags.NonPublic | BindingFlags.Instance)!.
                SetValue(ppu, (ushort)0x8000);
            typeof(PPU).GetField("_attributeRegisterHi", BindingFlags.NonPublic | BindingFlags.Instance)!.
                SetValue(ppu, (ushort)0x8000);
            
            ppu.Run(1); // Draw the first pixel of the second line (first visible sprite scanline)
            
            var bufferField = typeof(PPU).GetField("_frameBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var buffer = (byte[])bufferField!.GetValue(ppu)!;
          
            // Only the background color should be rendered
            const int pxOffset = 1 * 256 * 3; // Scanline 1, Cycle 0
            var expectedBgColor = PPU.Palette[0x15];
            buffer[pxOffset + 2].Should().Be(expectedBgColor.R); // R (228)
            buffer[pxOffset + 1].Should().Be(expectedBgColor.G); // G (0)
            buffer[pxOffset + 0].Should().Be(expectedBgColor.B); // B (88)
        }
        
        
        // TODO: do not render sprites if sprite drawing is disabled
        // TODO: sprite 0 hit
    }
}
