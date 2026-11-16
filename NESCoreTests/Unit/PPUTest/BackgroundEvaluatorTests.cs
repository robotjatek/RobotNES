using Moq;
using NESCore.PPU;

namespace NESCoreTests.Unit.PPUTest;

public class BackgroundEvaluatorTests : TestBase
{
    [Fact]
    public void ShouldFetchNametableOnPhase1()
    {
        var loopy = new LoopyRegisters();
        loopy.WriteAddress(0x20);
        loopy.WriteAddress(0x42);

        var registers = new PPURegisters { Mask = 0x8 };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x2042)).Returns(0x77);

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        // Phase 1 is at cycle 1
        bgEvaluator.Tick(1);

        memory.Verify(m => m.Read(0x2042), Times.Once);
    }

    [Fact]
    public void ShouldFetchAttributeTableOnPhase3()
    {
        var loopy = new LoopyRegisters();
        loopy.WriteAddress(0x00);
        loopy.WriteAddress(0x8C);

        var registers = new PPURegisters { Mask = 0x8 };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x208C)).Returns(0x00);
        memory.Setup(m => m.Read(0x23CB)).Returns(0xAA);

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        // Tick through cycles 0..3 (cycle 3 is phase 3, AT fetch)
        for (var cycle = 0; cycle <= 3; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        memory.Verify(m => m.Read(0x23CB), Times.Once);
    }

    [Fact]
    public void ShouldFetchPatternTableLowOnPhase5()
    {
        var loopy = new LoopyRegisters();
        loopy.WriteAddress(0x00);
        loopy.WriteAddress(0x8C);

        var registers = new PPURegisters
        {
            Control = 0x10, // BG pattern table in the second table (0x1000-0x1fff)
            Mask = 0x8
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x208C)).Returns(0x41); // Tile ID
        memory.Setup(m => m.Read(0x23CB)).Returns(0xAA);
        memory.Setup(m => m.Read(0x1410)).Returns(0b01010101); // PT low

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        // Tick through cycles 0..5 (phase 5 is PT low read)
        for (var cycle = 0; cycle <= 5; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        // 0x1000 + 0x41 (65) * 16 (+ fineY) => 0x1410
        memory.Verify(m => m.Read(0x1410), Times.Once);
    }

    [Fact]
    public void ShouldFetchPatternTableHighOnPhase7()
    {
        var loopy = new LoopyRegisters();
        loopy.WriteAddress(0x00);
        loopy.WriteAddress(0x8C);

        var registers = new PPURegisters
        {
            Control = 0x10, // BG pattern table in the second table (0x1000-0x1fff)
            Mask = 0x8
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x208C)).Returns(0x41); // Tile ID
        memory.Setup(m => m.Read(0x23CB)).Returns(0xAA);
        memory.Setup(m => m.Read(0x1410)).Returns(0b01010101);
        memory.Setup(m => m.Read(0x1418)).Returns(0b11001100); // PT high

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        for (var cycle = 0; cycle <= 7; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        // 0x1000 + 0x41 (65) * 16 (+ fineY) + 8 => 0x1418
        memory.Verify(m => m.Read(0x1418), Times.Once);
    }

    [Fact]
    public void NextPixel_ShouldProduceExpectedPixelsFromPatternTableAndAttributeTable()
    {
        // Tests the full shift register loading, shifting, attribute decoding and palette lookup
        // NT tile ID: 0x41, AT: 0b11_11_11_11 (palette 3)
        // PT low:  0b11110000
        // PT high: 0b10101010
        // Expected 8 pixels color indices:
        // bit 7: (1<<1)|1 = 3 -> Palette 3, Color 3 => 0x3F0F
        // bit 6: (0<<1)|1 = 1 -> Palette 3, Color 1 => 0x3F0D
        // bit 5: (1<<1)|1 = 3 -> Palette 3, Color 3 => 0x3F0F
        // bit 4: (0<<1)|1 = 1 -> Palette 3, Color 1 => 0x3F0D
        // bit 3: (1<<1)|0 = 2 -> Palette 3, Color 2 => 0x3F0E
        // bit 2: (0<<1)|0 = 0 -> Color 0 (transparent) => 0x3F00
        // bit 1: (1<<1)|0 = 2 -> Palette 3, Color 2 => 0x3F0E
        // bit 0: (0<<1)|0 = 0 -> Color 0 (transparent) => 0x3F00
        var loopy = new LoopyRegisters();
        loopy.WriteAddress(0x00);
        loopy.WriteAddress(0x8C);

        var registers = new PPURegisters
        {
            Control = 0x10,
            Mask = PPUMaskFlagPositions.SHOW_BACKGROUND | PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x208C)).Returns(0x41);
        memory.Setup(m => m.Read(0x23CB)).Returns(0b11111111);
        memory.Setup(m => m.Read(0x1410)).Returns(0b11110000); // PT low
        memory.Setup(m => m.Read(0x1418)).Returns(0b10101010); // PT high

        memory.Setup(m => m.Read(0x3F0D)).Returns(0x01); // Color for index 1
        memory.Setup(m => m.Read(0x3F0E)).Returns(0x02); // Color for index 2
        memory.Setup(m => m.Read(0x3F0F)).Returns(0x15); // Color for index 3
        memory.Setup(m => m.Read(0x3F00)).Returns(0x00); // Backdrop color

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        // Load tile in first 8 cycles (0..7)
        for (var cycle = 0; cycle < 8; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        // Shift 8 cycles so the loaded tile is shifted into the upper 8 bits
        for (var cycle = 8; cycle < 16; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        // Evaluate all 8 pixels
        // Pixel 0 (bit 7): color index 3
        var px0 = bgEvaluator.NextPixel(0);
        px0.opaque.Should().BeTrue();
        px0.pixel.Should().Be(Ppu.Palette[0x15]);
        bgEvaluator.Tick(0);

        // Pixel 1 (bit 6): color index 1
        var px1 = bgEvaluator.NextPixel(1);
        px1.opaque.Should().BeTrue();
        px1.pixel.Should().Be(Ppu.Palette[0x01]);
        bgEvaluator.Tick(1);

        // Pixel 2 (bit 5): color index 3
        var px2 = bgEvaluator.NextPixel(2);
        px2.opaque.Should().BeTrue();
        px2.pixel.Should().Be(Ppu.Palette[0x15]);
        bgEvaluator.Tick(2);

        // Pixel 3 (bit 4): color index 1
        var px3 = bgEvaluator.NextPixel(3);
        px3.opaque.Should().BeTrue();
        px3.pixel.Should().Be(Ppu.Palette[0x01]);
        bgEvaluator.Tick(3);

        // Pixel 4 (bit 3): color index 2
        var px4 = bgEvaluator.NextPixel(4);
        px4.opaque.Should().BeTrue();
        px4.pixel.Should().Be(Ppu.Palette[0x02]);
        bgEvaluator.Tick(4);

        // Pixel 5 (bit 2): color index 0 (transparent)
        var px5 = bgEvaluator.NextPixel(5);
        px5.opaque.Should().BeFalse();
        px5.pixel.Should().Be(Ppu.Palette[0x00]);
        bgEvaluator.Tick(5);

        // Pixel 6 (bit 1): color index 2
        var px6 = bgEvaluator.NextPixel(6);
        px6.opaque.Should().BeTrue();
        px6.pixel.Should().Be(Ppu.Palette[0x02]);
        bgEvaluator.Tick(6);

        // Pixel 7 (bit 0): color index 0 (transparent)
        var px7 = bgEvaluator.NextPixel(7);
        px7.opaque.Should().BeFalse();
        px7.pixel.Should().Be(Ppu.Palette[0x00]);
    }

    [Fact]
    public void ShouldReloadShiftRegistersFromPatternTable()
    {
        // Port of the old PpuTests.ShouldReloadShiftRegisters: Tick(7) must load the
        // low/high pattern bytes into the bottom of the shift registers. Asserted through
        // the public NextPixel API only. Tile 0x41 at (coarse X=12, coarse Y=0), pattern
        // row $1410/$1418, BG table selected by Control bit 4 (0x10 -> $1000).
        // pattern low $1410 = 11110000, pattern high $1418 = 10101010:
        //   pixel bit 7 -> (low 1, high 1) = color index 3 (bottom of the register after 16 ticks)
        //   pixel bit 6 -> (low 1, high 0) = color index 1
        var loopy = new LoopyRegisters();
        loopy.WriteAddress(0x00);
        loopy.WriteAddress(0x8C); // coarse X = 12, coarse Y = 0, fine Y = 0

        var registers = new PPURegisters
        {
            Control = 0x10, // BG pattern table at $1000
            Mask = PPUMaskFlagPositions.SHOW_BACKGROUND | PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x208C)).Returns(0x41);
        memory.Setup(m => m.Read(0x1410)).Returns(0b11110000); // pattern low
        memory.Setup(m => m.Read(0x1418)).Returns(0b10101010); // pattern high
        memory.Setup(m => m.Read(0x3F00)).Returns(0x0F);
        memory.Setup(m => m.Read(0x3F03)).Returns(0x15); // palette 0, color 3 (attr byte 0)

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);
        for (var cycle = 0; cycle < 16; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        var (opaque, pixel) = bgEvaluator.NextPixel(8);
        opaque.Should().BeTrue();
        pixel.Should().Be(Ppu.Palette[0x15]); // palette 3, color 3 = loaded pattern bit 7
    }

    [Fact]
    public void ShouldShiftShiftRegistersLeftOneBitPerTick()
    {
        // Port of the old PpuTests.ShiftRegisters_ShouldShiftLeftOneBit_DuringVisibleCycles:
        // the shift registers advance left by one bit every Tick. Compared with the load test,
        // one extra Tick here shifts the loaded pattern one position up, so the first pixel is
        // the second pixel of the tile: pattern 11110000 / 10101010 -> index 3 -> palette 3.
        var loopy = new LoopyRegisters();
        loopy.WriteAddress(0x00);
        loopy.WriteAddress(0x8C); // coarse X = 12, coarse Y = 0, fine Y = 0

        var registers = new PPURegisters
        {
            Control = 0x10,
            Mask = PPUMaskFlagPositions.SHOW_BACKGROUND | PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x208C)).Returns(0x41);
        memory.Setup(m => m.Read(0x1410)).Returns(0b11110000);
        memory.Setup(m => m.Read(0x1418)).Returns(0b10101010);
        memory.Setup(m => m.Read(0x3F00)).Returns(0x0F);
        memory.Setup(m => m.Read(0x3F03)).Returns(0x15);

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);
        for (var cycle = 0; cycle < 16; cycle++)
        {
            bgEvaluator.Tick(cycle);   // load at 7; register full (11110000/10101010) at end of 15
        }
        bgEvaluator.Tick(16);          // 17th tick = the left-shift being asserted (no reload at phase 7)
        bgEvaluator.Tick(17);          // advance so the shifted pixel reaches the read position

        var (opaque, pixel) = bgEvaluator.NextPixel(8);
        opaque.Should().BeTrue();
        pixel.Should().Be(Ppu.Palette[0x15]); // palette 0, color 3 = shifted-in pattern pixel bit 6
    }

    [Fact]
    public void ShouldDecodeCorrectAttributeQuadrant()
    {
        // Verifies the bottom-right attribute quadrant (bits 6,7 of the AT byte) is decoded,
        // observed purely through the public NextPixel API. Only the bottom-right 2 bits are set
        // to palette 3, so a lit pixel can only come from that quadrant. NOTE: with the current
        // GetAttributeShift implementation the bottom-right pair is selected only when BOTH
        // coarse-X bit1 and coarse-Y bit1 are set (the quadrant bits are combined before masking),
        // so coarse X and coarse Y are both positioned to 2 here.
        
        // coarse X and coarse y determine which quarter of the AT to use
        // coarse X bit 1 == 1 => right
        // coarse Y bit 1 == 1 => bottom
        var loopy = new LoopyRegisters();
        for (var i = 0; i < 2; i++) loopy.IncrementCoarseX(); // coarse X = 2
        for (var i = 0; i < 16; i++) loopy.IncrementY();       // coarse Y = 2, fineY = 0 (V = 0x0042)

        var registers = new PPURegisters
        {
            Control = 0x00,
            Mask = PPUMaskFlagPositions.SHOW_BACKGROUND | PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN
        };

        // Tile 1 at (coarse X=2, coarse Y=2) => nametable $2042, pattern row $0010/$0018 (fineY 0).
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x2042)).Returns(1);
        // Attribute byte: palette 3 (11) in the bottom-right quadrant (bits 6,7); others 00.
        memory.Setup(m => m.Read(0x23C0)).Returns(0b11000000);
        memory.Setup(m => m.Read(0x0010)).Returns(0b10000000); // PT low  -> color index 1
        memory.Setup(m => m.Read(0x0018)).Returns(0b00000000); // PT high -> color index 1
        memory.Setup(m => m.Read(0x3F0D)).Returns(0x25);       // palette 3, color 1
        memory.Setup(m => m.Read(0x3F00)).Returns(0x0F);       // backdrop

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        // Tick a tile's worth of cycles so the pattern loaded at Tick(7) shifts up to where
        // the pixel is read, then read the pixel back through the public API only.
        for (var cycle = 0; cycle < 16; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        var (opaque, pixel) = bgEvaluator.NextPixel(8);
        opaque.Should().BeTrue();
        pixel.Should().Be(Ppu.Palette[0x25]);
    }

    [Fact]
    public void NextPixel_ShouldRenderDefaultBackground_WhenTransparent()
    {
        // When color index is 0 (transparent), palette address 0x3F00 is read
        var loopy = new LoopyRegisters();
        var registers = new PPURegisters
        {
            Mask = PPUMaskFlagPositions.SHOW_BACKGROUND | PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x3F00)).Returns(0x20);

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        var (opaque, pixel) = bgEvaluator.NextPixel(0);

        opaque.Should().BeFalse();
        pixel.Should().Be(Ppu.Palette[0x20]);
    }

    [Fact]
    public void NextPixel_ShouldRenderDefaultBackground_WhenBackgroundRenderingDisabled()
    {
        var loopy = new LoopyRegisters();
        var registers = new PPURegisters { Mask = 0 }; // Rendering disabled
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x3F00)).Returns(0x0F);

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        var (opaque, pixel) = bgEvaluator.NextPixel(0);

        opaque.Should().BeFalse();
        pixel.Should().Be(Ppu.Palette[0x0F]);
    }

    [Fact]
    public void NextPixel_ShouldHideBackgroundInLeft8Pixels_WhenLeftColumnDisabled()
    {
        var loopy = new LoopyRegisters();
        var registers = new PPURegisters
        {
            Mask = PPUMaskFlagPositions.SHOW_BACKGROUND // Left column flag NOT set
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x3F00)).Returns(0x0F);

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        // At cycle 7 (< 8), left column is hidden -> default background rendered
        var (opaque7, pixel7) = bgEvaluator.NextPixel(7);
        opaque7.Should().BeFalse();
        pixel7.Should().Be(Ppu.Palette[0x0F]);
    }

    [Fact]
    public void NextPixel_ShouldMapPaletteIndexToNtscRgb()
    {
        // Ported from PPUTests.OnVisibleScanline_ShouldRenderPixelFromPalette: the background
        // pipeline must resolve a palette index to the exact NTSC RGB triple from Ppu.Palette.
        // Palette index 0x15 is NTSC (228, 0, 88).
        var loopy = new LoopyRegisters();

        var registers = new PPURegisters
        {
            Mask = PPUMaskFlagPositions.SHOW_BACKGROUND | PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN
        };

        // Make the first pixel opaque with color index 1 in palette 0, whose entry is 0x15.
        // Nametable at V=0 -> 0x2000 -> tile 1; bg pattern base 0x0000 -> planes 0x0010/0x0018;
        // attribute 0 -> palette 0; the palette read is 0x3F01.
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(0x2000)).Returns(1);   // tile 1
        memory.Setup(m => m.Read(0x23C0)).Returns(0x00); // attribute: palette 0
        memory.Setup(m => m.Read(0x0010)).Returns(0b10000000); // plane low  -> color index 1
        memory.Setup(m => m.Read(0x0018)).Returns(0b00000000); // plane high -> color index 1
        memory.Setup(m => m.Read(0x3F01)).Returns(0x15);        // palette 0, color 1 -> NTSC index 0x15
        memory.Setup(m => m.Read(0x3F00)).Returns(0x0F);        // backdrop

        var bgEvaluator = new BackgroundEvaluator(loopy, memory.Object, registers);

        // Tick a tile's worth of cycles so the pattern loaded at Tick(7) shifts up to where
        // the pixel is read, then read the pixel back through the public API only.
        for (var cycle = 0; cycle < 16; cycle++)
        {
            bgEvaluator.Tick(cycle);
        }

        var (opaque, pixel) = bgEvaluator.NextPixel(8);

        opaque.Should().BeTrue();
        pixel.Should().Be(Ppu.Palette[0x15]);
        pixel.R.Should().Be(228);
        pixel.G.Should().Be(0);
        pixel.B.Should().Be(88);
    }
}
