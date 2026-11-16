using Moq;

using NESCore.PPU;

using Serilog;
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace NESCoreTests.Unit.PPUTest
{
    public class PpuTests : TestBase
    {
        [Fact]
        public void ReadFromStatusOnlyReturnsTheTop3bits()  //TODO: openbus behaviour. Return previous data from the ppu databuffer on the lower bits
        {
            const byte testData = 0xff;
            var ppuRegisters = new PPURegisters
            {
                Status = testData
            };

            var ppu = new Ppu(ppuRegisters, _ppuMemory.Object, _logger);
            var data = ppu.Read(0x2002);
            data.Should().Be(0b11100000);
        }

        [Fact]
        public void ReadFromStatusResetsVBlankFlag()
        {
            const byte testData = 0xE0;
            var ppuRegisters = new PPURegisters
            {
                Status = testData
            };
            var ppu = new Ppu(ppuRegisters, _ppuMemory.Object, _logger);
            var status = ppu.Read(0x2002);
            status.Should().Be(0b11100000);
            ppuRegisters.Status.Should().Be(0b01100000);
        }

        [Fact]
        public void SetsVBlankFlagAndSendsNmi()
        {
            var registers = new PPURegisters
            {
                Control = 0x80 //Enable nmi
            };
            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            const uint beforeVBlank =
                341 * 240 // Visible screen
                + 341; // Post render scanline

            ppu.Run(beforeVBlank);
            registers.GetVBlankFlag().Should().BeFalse();
            ppu.Run(2); // set the vblank flag on the first cycle of the vblank lines

            registers.GetVBlankFlag().Should().BeTrue();
            eventMonitor.Should().Raise(nameof(ppu.NMIEvent));
        }

        [Fact]
        public void SetsVBlankFlagButDoesNotSendNmi()
        {
            var registers = new PPURegisters();
            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            const uint beforeVBlank =
                341 * 240 // Visible screen
                + 341; // Post render scanline

            ppu.Run(beforeVBlank);
            registers.GetVBlankFlag().Should().BeFalse();
            ppu.Run(2); // set the vblank flag on the first cycle of the vblank lines

            registers.GetVBlankFlag().Should().BeTrue();
            eventMonitor.Should().NotRaise(nameof(ppu.NMIEvent));
        }

        [Fact]
        public void SetsVBlankFlagAndSendsNmiMultipleTimes()
        {
            var registers = new PPURegisters
            {
                Control = 0x80 //Enable nmi
            };
            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
            using var eventMonitor = ppu.Monitor();

            for (var i = 0; i < 3; i++) // Three times a charm!
            {
                const uint frameCycles = 341 * 261; // Visible screen
                ppu.Run(frameCycles);

                registers.GetVBlankFlag().Should().BeTrue();
            }

            eventMonitor.OccurredEvents.Should().HaveCount(3);
        }

        [Fact]
        public void ClearsVBlankFlag()
        {
            var registers = new PPURegisters();
            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
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

            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
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

            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
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

            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
            ppu.Write(0x2000, 0xff);

            registers.Control.Should().Be(0xff);
        }

        [Fact]
        public void ReadFromControlRegisterLogs()
        {
            var registers = new PPURegisters();
            var logger = new Mock<ILogger>();
            var ppu = new Ppu(registers, _ppuMemory.Object, logger.Object);
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
            var ppu = new Ppu(registers, _ppuMemory.Object, _logger);
            ppu.Write(0x2001, 0xff);

            registers.Mask.Should().Be(0xff);
        }

        [Fact]
        public void ReadFromMaskRegisterLogs()
        {
            var registers = new PPURegisters();
            var logger = new Mock<ILogger>();
            var ppu = new Ppu(registers, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2001);

            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void ReadFromScrollLogs()
        {
            var logger = new Mock<ILogger>();
            var ppu = new Ppu(_ppuRegisters.Object, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2005);

            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void ReadFromAddressLogs()
        {
            var logger = new Mock<ILogger>();
            var ppu = new Ppu(_ppuRegisters.Object, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2006);

            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void WriteToPpuRam()
        {
            var ppuMemory = new Mock<IPPUMemory>();

            var registers = new PPURegisters();
            var ppu = new Ppu(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second
            ppu.Write(0x2007, 0xaa); // Data byte

            ppuMemory.Verify(m => m.Write(0x3c0f, 0xaa), Times.Once());
        }

        [Fact]
        public void WriteToPpuRamIncrementsAddressByOne()
        {
            var ppuMemory = new Mock<IPPUMemory>();

            var registers = new PPURegisters();
            var ppu = new Ppu(registers, ppuMemory.Object, _logger);
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
        public void WriteToPpuRamIncrementsAddressBy32()
        {
            var ppuMemory = new Mock<IPPUMemory>();

            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.VRAMIncrement()).Returns(32);

            var ppu = new Ppu(registers.Object, ppuMemory.Object, _logger);
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
        public void ReadFromPpuRam()
        {
            var ppuMemory = new Mock<IPPUMemory>();
            ppuMemory.Setup(m => m.Read(0x3c0f)).Returns(0xaa);

            var registers = new PPURegisters();
            var ppu = new Ppu(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            ppu.Read(0x2007); //First result is the previously buffered data
            var result = ppu.Read(0x2007); // real data byte

            result.Should().Be(0xaa);
            ppuMemory.Verify(m => m.Read(0x3c0f), Times.Once());
        }

        [Fact]
        public void ReadFromPpuRamPaletteIsNotBuffered()
        {
            var ppuMemory = new Mock<IPPUMemory>();
            ppuMemory.Setup(m => m.Read(0x3f00)).Returns(0xaa);

            var registers = new PPURegisters();
            var ppu = new Ppu(registers, ppuMemory.Object, _logger);
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
            var ppu = new Ppu(registers, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            ppu.Read(0x2007); //First read should be discared because of the buffering

            var result = ppu.Read(0x2007);
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
        public void ReadFromPpuRamIncrementsAddressBy32()
        {
            var ppuMemory = new Mock<IPPUMemory>();
            ppuMemory.Setup(m => m.Read(0x3c0e)).Returns(0x00);
            ppuMemory.Setup(m => m.Read(0x3c0f)).Returns(0xaa);
            ppuMemory.Setup(m => m.Read(0x3c2f)).Returns(0xab);
            ppuMemory.Setup(m => m.Read(0x3c4f)).Returns(0xac);

            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.VRAMIncrement()).Returns(32);

            var ppu = new Ppu(registers.Object, ppuMemory.Object, _logger);
            ppu.Write(0x2006, 0x3c); // Upper byte first
            ppu.Write(0x2006, 0x0f); // Lower byte second

            ppu.Read(0x2007); //First read should be discared because of the buffering

            var result = ppu.Read(0x2007);
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
        public void ReadFromOamAddressLogs()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();

            var logger = new Mock<ILogger>();

            var ppu = new Ppu(registers.Object, _ppuMemory.Object, logger.Object);
            ppu.Read(0x2003);
            logger.Verify(l => l.Warning(It.IsAny<string>()));
        }

        [Fact]
        public void WriteToOamAddress()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            
            var ppu = new Ppu(registers.Object, _ppuMemory.Object, _logger);
            ppu.Write(0x2003, 0xaa);
            registers.Object.OAMAddress.Should().Be(0xaa);
        }

        [Fact]
        public void WriteToOamDataWritesValueToOamAndIncrementsOamAddress()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Object.OAMAddress = 0x0;

            var memory = new Mock<IPPUMemory>();

            var ppu = new Ppu(registers.Object, memory.Object, _logger);
            ppu.Write(0x2004, 0xde);

            memory.Verify(m => m.OamWrite(0x0, 0xde));
            registers.Object.OAMAddress.Should().Be(1);
        }

        [Fact]
        public void ReadFromOamDataReadsValueFromOamAndDoesNotIncrementOamAddress()
        {
            var registers = new Mock<IPPURegisters>();
            registers.SetupAllProperties();
            registers.Object.OAMAddress = 0x0;

            var memory = new Mock<IPPUMemory>();
            memory.Setup(m => m.OamRead(0)).Returns(0xde);

            var ppu = new Ppu(registers.Object, memory.Object, _logger);
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
            var ppu = new Ppu(registers, memory.Object, _logger);

            // V = 2042
            ppu.Write(0x2006, 0x20);
            ppu.Write(0x2006, 0x42);
            memory.Setup(m => m.Read(0x2042)).Returns(0x77);
            
            ppu.Run(2);
            
            memory.Verify(m => m.Read(0x2042), Times.Once);
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
            var ppu = new Ppu(registers, memory.Object, _logger);
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
            var ppu = new Ppu(registers, memory.Object, _logger);
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
        public void ShouldKeepFetchingOnNonVisibleCycles()
        {
            // ppu does not stop fetching during non-visible cycles
            // NT, AT, PATTERN1, PATTERN2 fetches keep happening after between cycle 320 and 340 to prefetch the next 2 tiles for the next scanline

            var registers = new PPURegisters { Control = 0x10, Mask = 0x8 };
            var memory = new Mock<IPPUMemory>();
            var ppu = new Ppu(registers, memory.Object, _logger);
            
            ppu.Write(0x2006, 0x20);
            ppu.Write(0x2006, 0x0);
            memory.Setup(m => m.Read(0x2000)).Returns(0x99);
            memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0);
            
            ppu.Run(321); // the first pre-fetch nt read should happen here
            
            memory.Verify(m => m.Read(0x2000), Times.Once);
        }

        [Fact]
        public void ShouldSetVBlankFlagAtTheExactStartOfVBlank()
        {
            var registers = new PPURegisters { Control = 0x80 }; // nmi enabled: bit 7 = 1
            var memory = new Mock<IPPUMemory>();
            var ppu = new Ppu(registers, memory.Object, _logger);
            using var monitor = ppu.Monitor();

            uint cyclesBeforeVBlank = 240 * 341 + 341;
            ppu.Run(cyclesBeforeVBlank);
            registers.GetVBlankFlag().Should().BeFalse();

            ppu.Run(1);

            registers.GetVBlankFlag().Should().BeTrue();
            monitor.Should().Raise(nameof(ppu.NMIEvent));
        }

        
        [Fact]
        public void ShouldRenderBackgroundInsteadOfSprite()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprites on 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES |
                       PPUMaskFlagPositions.SHOW_BACKGROUND |
                       PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN |
                       PPUMaskFlagPositions.SHOW_SPRITES_IN_LEFT_COLUMN
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new Ppu(registers, memory.Object, _logger);
            
            // Sprite 0 covering scanline 8 (OamEntry.ScreenY = Y + 1, so Y=7 draws rows 8..15),
            // X = 0-7, Palette 3, BEHIND the background.
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        0 => new OamEntry(7, 0, new OamEntry.SpriteAttribute(3, BehindBackground: true, false, false).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });
            
            const byte pattern1 = 0b10000000;
            const byte pattern2 = 0b01000000;
            // Both layers opaque at scanline 8. Bg pattern table base 0x0000, tile 1 planes
            // 0x0010/0x0018 all set => bg color index 3 (opaque), attribute 0 => palette 0 => 0x3F03.
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // sprite 0 pattern row 0 (sprite ScreenY=8 => row = scanline - 8)
                        0x1000 => pattern1,
                        0x1008 => pattern2,

                        // opaque background tile 1 at the scanline-8 fetch addresses
                        0x2020 => 1,
                        0x2021 => 1,
                        0x2000 => 1,
                        0x2001 => 1,
                        0x0010 => 0xFF, // tile 1 plane low
                        0x0018 => 0xFF, // tile 1 plane high
                        0x23C0 => 0x00, // attribute: palette 0
                        0x3F03 => 0x0F, // palette 0, color index 3 => background color

                        // palette px 1 (first sprite pixel from palette index 4)
                        0x3F1D => 4,
                        
                        _ => 0
                    };
                });
            
            ppu.Run(341); // First line
            
            // Land on scanline 8, cycle 8 where both layers are opaque; the sprite is flagged
            // BehindBackground, so the opaque background pixel must win.
            ppu.Run(341 * 7 + 9); // scanline 8, cycle 8
            
            var buffer = ppu.FrameBuffer;
          
            // Only the background color should be rendered
            const int pxOffset = 8 * 256 * 3; // Scanline 8, Cycle 0
            var expectedBgColor = Ppu.Palette[0x0F];
            buffer[pxOffset + 2].Should().Be(expectedBgColor.R);
            buffer[pxOffset + 1].Should().Be(expectedBgColor.G);
            buffer[pxOffset + 0].Should().Be(expectedBgColor.B);
        }
        
        [Fact]
        public void ShouldRenderSpriteInsteadOfBackground()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprites on 0x1000
                Mask = PPUMaskFlagPositions.SHOW_SPRITES |
                       PPUMaskFlagPositions.SHOW_BACKGROUND |
                       PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN | 
                       PPUMaskFlagPositions.SHOW_SPRITES_IN_LEFT_COLUMN
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new Ppu(registers, memory.Object, _logger);
            
            // Sprite 0 covering scanline 8 (OamEntry.ScreenY = Y + 1, so Y=7 draws rows 8..15),
            // X = 0-7, Palette 3, IN FRONT of the background.
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        0 => new OamEntry(7, 0, new OamEntry.SpriteAttribute(3, BehindBackground: false, false, false).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });
            
            const byte pattern1 = 0b10000000;
            const byte pattern2 = 0b01000000;
            // Both layers opaque at scanline 8; the sprite is in front so it must win.
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // sprite 0 pattern row 0 (sprite ScreenY=8 => row = scanline - 8)
                        0x1000 => pattern1,
                        0x1008 => pattern2,

                        // opaque background tile 1 at the scanline-8 fetch addresses
                        0x2020 => 1,
                        0x2021 => 1,
                        0x2000 => 1,
                        0x2001 => 1,
                        0x0010 => 0xFF, // tile 1 plane low
                        0x0018 => 0xFF, // tile 1 plane high
                        0x23C0 => 0x00, // attribute: palette 0
                        0x3F03 => 0x0F, // palette 0, color index 3 => background color

                        // palette px 1 (first sprite pixel from palette index 4)
                        0x3F1D => 4,
                        
                        _ => 0
                    };
                });
            
            ppu.Run(341); // First line
            
            // Land on scanline 8, cycle 8 where both layers are opaque; the sprite is in front.
            ppu.Run(341 * 7 + 9); // scanline 8, cycle 8
            
            var buffer = ppu.FrameBuffer;
          
            // The sprite color should win over the background
            const int pxOffset = 8 * 256 * 3; // Scanline 8, Cycle 0
            var expectedBgColor = Ppu.Palette[4];
            buffer[pxOffset + 2].Should().Be(expectedBgColor.R);
            buffer[pxOffset + 1].Should().Be(expectedBgColor.G);
            buffer[pxOffset + 0].Should().Be(expectedBgColor.B);
        }
        
        [Fact]
        public void ShouldNotRenderSpiteWhenSpriteRenderingIsDisabled()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprites on 0x1000
                Mask =  PPUMaskFlagPositions.SHOW_BACKGROUND | PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN // No sprite rendering
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new Ppu(registers, memory.Object, _logger);
            
            // Sprite setup:
            // There is a sprite at X = 0-7, Palette 3 covering scanline 8 (Y=7 => screen row 8),
            // but sprite rendering is disabled in the mask, so the background must be rendered.
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        0 => new OamEntry(7, 0, new OamEntry.SpriteAttribute(3, BehindBackground: false, false, false).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });
            
            const byte pattern1 = 0b10000000;
            const byte pattern2 = 0b01000000;
            // Bg opaque at scanline 8 (tile 1 => color index 3 => palette 0 => 0x3F03).
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // sprite 0 pattern row 0 (sprite ScreenY=8 => row = scanline - 8)
                        0x1000 => pattern1,
                        0x1008 => pattern2,

                        // opaque background tile 1 at the scanline-8 fetch addresses
                        0x2020 => 1,
                        0x2021 => 1,
                        0x2000 => 1,
                        0x2001 => 1,
                        0x0010 => 0xFF, // tile 1 plane low
                        0x0018 => 0xFF, // tile 1 plane high
                        0x23C0 => 0x00, // attribute: palette 0
                        0x3F03 => 0x0F, // palette 0, color index 3 => background color

                        // palette px 1 (first sprite pixel from palette index 4)
                        0x3F1D => 4,
                        
                        _ => 0
                    };
                });
            
            ppu.Run(341); // First line
            
            // Land on scanline 8, cycle 8. Sprite rendering is disabled, so even though a sprite
            // covers this pixel, only the background color should be rendered.
            ppu.Run(341 * 7 + 9); // scanline 8, cycle 8
            
            var buffer = ppu.FrameBuffer;
          
            // Only the background color should be rendered
            const int pxOffset = 8 * 256 * 3; // Scanline 8, Cycle 0
            var expectedBgColor = Ppu.Palette[0x0F];
            buffer[pxOffset + 2].Should().Be(expectedBgColor.R);
            buffer[pxOffset + 1].Should().Be(expectedBgColor.G);
            buffer[pxOffset + 0].Should().Be(expectedBgColor.B);
        }
        
        [Fact]
        public void ShouldSetSpriteZeroHitBit()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprites on 0x1000
                Mask =  PPUMaskFlagPositions.SHOW_BACKGROUND | 
                        PPUMaskFlagPositions.SHOW_SPRITES |
                        PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN |
                        PPUMaskFlagPositions.SHOW_SPRITES_IN_LEFT_COLUMN
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new Ppu(registers, memory.Object, _logger);
            
            // Sprite setup:
            // Sprite 0 covering scanline 8 (OamEntry.ScreenY = Y + 1, so Y=7 draws on rows 8..15),
            // X = 0-7, Palette 3, in front of the background.
            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => 
                { 
                    return i switch 
                    { 
                        0 => new OamEntry(7, 0, new OamEntry.SpriteAttribute(3, BehindBackground: false, false, false).ToByte(), 0, i), 
                        _ => new OamEntry(0xEF, 0, 0, 10, i) 
                    }; 
                });
            
            const byte pattern1 = 0b10000000;
            const byte pattern2 = 0b01000000;
            // Background opaque on scanline 8: the fetch (V coarseY=1, coarseX 0/1) reads the nametable
            // at 0x2020/0x2021 -> tile 1. Bg pattern base 0x0000, tile 1 planes 0x0010/0x0018 all set =>
            // color index 3 (opaque). Attribute 0 => palette 0 => NextPixel reads 0x3F03.
            memory.Setup(m => m.Read(It.IsAny<ushort>()))
                .Returns((ushort address) =>
                {
                    return address switch
                    {
                        // sprite 0 pattern row 0 (sprite ScreenY=8 => row = scanline - 8)
                        0x1000 => pattern1,
                        0x1008 => pattern2,

                        // tile 1 (opaque, color index 3) at every coarseX the fetch may hit
                        0x2020 => 1,
                        0x2021 => 1,
                        0x2000 => 1,
                        0x2001 => 1,
                        0x0010 => 0xFF, // tile 1 plane low
                        0x0018 => 0xFF, // tile 1 plane high
                        0x23C0 => 0x00, // attribute: palette 0
                        0x3F03 => 0x0F, // palette 0, color index 3 => opaque

                        // palette px 1 (first sprite pixel from palette index 4)
                        0x3F1D => 4,

                        _ => 0
                    };
                });

            ppu.Run(341); // First line
            registers.GetSpriteZeroHitFlag().Should().BeFalse();

            // Land on scanline 8, cycle 8: bg shift registers reloaded with the opaque tile, sprite 0 covers x=0..7,
            // so the pixel is sprite 0 over an opaque background -> the sprite-0 hit flag gets set.
            ppu.Run(341 * 7 + 9); // scanline 8, cycle 8

            registers.GetSpriteZeroHitFlag().Should().BeTrue();
        }
        
        [Fact]
        public void ShouldRenderSpriteBehindBackground_WhenBackgroundIsTransparent()
        {
            var registers = new PPURegisters()
            {
                Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS,
                Mask = PPUMaskFlagPositions.SHOW_SPRITES |
                       PPUMaskFlagPositions.SHOW_BACKGROUND | 
                       PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN |
                       PPUMaskFlagPositions.SHOW_SPRITES_IN_LEFT_COLUMN
            };
            var memory = new Mock<IPPUMemory>();
            var ppu = new Ppu(registers, memory.Object, _logger);

            memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
                .Returns((int i) => i switch
                {
                    0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(3, BehindBackground: true, false, false).ToByte(), 0, i),
                    _ => new OamEntry(0xEF, 0, 0, 10, i)
                });

            memory.Setup(m => m.Read(0x1000)).Returns(0b10000000); // sprite pattern
            memory.Setup(m => m.Read(0x3F1D)).Returns(4);          // sprite color
            memory.Setup(m => m.Read(0x3F00)).Returns(0x22);       // backdrop color

            ppu.Run(341); // scanline 0
            ppu.Run(1);   // scanline 1

            var buffer = ppu.FrameBuffer;

            // Sprite pixel should show through transparent background
            const int pxOffset = 1 * 256 * 3;
            var expectedSprColor = Ppu.Palette[4];
            buffer[pxOffset + 2].Should().Be(expectedSprColor.R);
            buffer[pxOffset + 1].Should().Be(expectedSprColor.G);
            buffer[pxOffset + 0].Should().Be(expectedSprColor.B);
        }
        
    }
}
