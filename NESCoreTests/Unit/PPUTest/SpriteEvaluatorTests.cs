using Moq;
using NESCore.PPU;

namespace NESCoreTests.Unit.PPUTest;

public class SpriteEvaluatorTests : TestBase
{
    [Fact]
    public void ShouldDrawIntoTheSpriteScanlineBuffer()
    {
        var registers = new PPURegisters()
        {
            Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // sprites on 0x1000
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) =>
            {
                return i switch
                {
                    // X = 0-7, Palette 3
                    0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(3, BehindBackground:false, false, false).ToByte(), 0, i),
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

        evaluator.EvaluateSprites(1); // first scanline with sprites

        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;
        spriteScanlineBuffer.Where(c => c.PixelColor != null).Should().HaveCount(3); // three visible pixels

        var expected = new List<SpriteEvaluator.SpritePixelData>();
        for (var i = 0; i < 256; i++)
        {
            expected.Add(new SpriteEvaluator.SpritePixelData(null, false));
        }
        expected[0] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[10], false, true);
        expected[1] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[13], false, true);
        expected[6] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[13], false, true);

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
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) =>
            {
                return i switch
                {
                    0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(1, BehindBackground: false, false, false).ToByte(), 0, i),
                    1 => new OamEntry(0, 1, new OamEntry.SpriteAttribute(3, BehindBackground: false, false, false).ToByte(), 0, i),
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

                    0x3f16 => 3, // PaletteID for sprite1
                    0x3f1E => 4, // PaletteID for sprite2
                    _ => 0
                };
            });

        evaluator.EvaluateSprites(1);

        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;
        spriteScanlineBuffer.Where(c => c.PixelColor != null).Should().HaveCount(8);

        var expected = new List<SpriteEvaluator.SpritePixelData>();
        for (var i = 0; i < 256; i++)
        {
            expected.Add(new SpriteEvaluator.SpritePixelData(null, false));
        }

        expected[0] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[1] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[2] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[3] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[4] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[5] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[6] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[7] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);

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
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) =>
            {
                return i switch
                {
                    0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(1, BehindBackground: false, false, true).ToByte(), 0, i),
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

        evaluator.EvaluateSprites(1);

        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;
        spriteScanlineBuffer.Where(c => c.PixelColor != null).Should().HaveCount(5);

        var expected = new List<SpriteEvaluator.SpritePixelData>();
        for (var i = 0; i < 256; i++)
        {
            expected.Add(new SpriteEvaluator.SpritePixelData(null, false));
        }

        expected[0] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[0], false, true);
        expected[1] = new SpriteEvaluator.SpritePixelData(null, false);
        expected[2] = new SpriteEvaluator.SpritePixelData(null, false);
        expected[3] = new SpriteEvaluator.SpritePixelData(null, false);
        expected[4] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[5] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[6] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[7] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);

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
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) =>
            {
                return i switch
                {
                    0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(1, BehindBackground:false, true, false).ToByte(), 0, i),
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

        evaluator.EvaluateSprites(1);

        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;
        spriteScanlineBuffer.Where(c => c.PixelColor != null).Should().HaveCount(5);

        var expected = new List<SpriteEvaluator.SpritePixelData>();
        for (var i = 0; i < 256; i++)
        {
            expected.Add(new SpriteEvaluator.SpritePixelData(null, false));
        }

        expected[0] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[1] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[2] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[3] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[3], false, true);
        expected[4] = new SpriteEvaluator.SpritePixelData(null, false);
        expected[5] = new SpriteEvaluator.SpritePixelData(null, false);
        expected[6] = new SpriteEvaluator.SpritePixelData(null, false);
        expected[7] = new SpriteEvaluator.SpritePixelData(Ppu.Palette[0], false, true);

        spriteScanlineBuffer.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldDraw8x16Sprite_TopAndBottomPatterns()
    {
        var registers = new PPURegisters()
        {
            Control = PPUControlFlagPositions.SPRITE_SIZE,
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) => i switch
            {
                0 => new OamEntry(0, 1, new OamEntry.SpriteAttribute(0, BehindBackground: false, false, false).ToByte(), 0, i),
                _ => new OamEntry(0xEF, 0, 0, 10, i)
            });

        // Upper pattern (pattern 0): 0x1000, row 0: plane low at 0x1000, plane high at 0x1008
        // Lower pattern (pattern 1): 0x1010, row 0: plane low at 0x1010, plane high at 0x1018
        const byte topPattern1 = 0b10000000;
        const byte topPattern2 = 0b00000000;
        const byte bottomPattern1 = 0b00000001;
        const byte bottomPattern = 0b00000001;

        memory.Setup(m => m.Read(It.IsAny<ushort>()))
            .Returns((ushort address) => address switch
            {
                0x1000 => topPattern1,
                0x1008 => topPattern2,

                0x1010 => bottomPattern1,
                0x1018 => bottomPattern,

                0x3F11 => 5, // Palette 0, color index 1
                0x3F13 => 9, // Palette 0, color index 3
                _ => 0
            });

        var buffer = evaluator.SpriteScanlineBuffer;

        // Scanline 1 (upper pattern, row 0)
        evaluator.EvaluateSprites(1);
        buffer[0].Should().Be(new SpriteEvaluator.SpritePixelData(Ppu.Palette[5], false, true));
        buffer[7].PixelColor.Should().BeNull();

        // Advance 8 scanlines to scanline 9 (lower pattern, row 0)
        evaluator.EvaluateSprites(9);
        buffer[7].Should().Be(new SpriteEvaluator.SpritePixelData(Ppu.Palette[9], false, true));
        buffer[0].PixelColor.Should().BeNull();

        // Advance 8 scanlines to scanline 17 (past the 16 pixel height)
        evaluator.EvaluateSprites(17);
        buffer.Where(c => c.PixelColor != null).Should().BeEmpty();
    }

    [Fact]
    public void ShouldSelectPatternTableBasedOnSpritePatternIndexBit0_In8x16Mode()
    {
        var registers = new PPURegisters()
        {
            Control = PPUControlFlagPositions.SPRITE_SIZE,
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) => i switch
            {
                0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(0, BehindBackground: false, false, false).ToByte(), 0, i),
                _ => new OamEntry(0xEF, 0, 0, 10, i)
            });

        // Bit 0 of TileIndex is 0, so the 0x0000 pattern table is used
        const byte pattern = 0b10000000;
        memory.Setup(m => m.Read(It.IsAny<ushort>()))
            .Returns((ushort address) => address switch
            {
                0x0000 => pattern,
                0x3F11 => 7,
                _ => 0
            });

        evaluator.EvaluateSprites(1);

        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;
        spriteScanlineBuffer[0].Should().Be(new SpriteEvaluator.SpritePixelData(Ppu.Palette[7], false, true));
    }

    [Fact]
    public void ShouldFlip8By16SpriteVertically()
    {
        var registers = new PPURegisters()
        {
            Control = PPUControlFlagPositions.SPRITE_SIZE,
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) => i switch
            {
                0 => new OamEntry(0, 0, new OamEntry.SpriteAttribute(0, BehindBackground: false, VerticalFlip: true, false).ToByte(), 0, i),
                _ => new OamEntry(0xEF, 0, 0, 10, i)
            });

        // When flipped vertically:
        // Scanline 1 (sprite row 0) reads the last row of the lower pattern (row 15 => pattern 1, row 7 at 0x0017)
        // Scanline 16 (sprite row 15) reads the first row of the upper pattern (row 0 => pattern 0, row 0 at 0x0000)
        const byte botRowPattern = 0b10000000;
        const byte topRowPattern = 0b00000001;

        memory.Setup(m => m.Read(It.IsAny<ushort>()))
            .Returns((ushort address) => address switch
            {
                0x0017 => botRowPattern,
                0x0000 => topRowPattern,
                0x3F11 => 5,
                _ => 0
            });

        // Scanline 1: the last row of the lower pattern should be rendered
        evaluator.EvaluateSprites(1);
        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;
        spriteScanlineBuffer[0].Should().Be(new SpriteEvaluator.SpritePixelData(Ppu.Palette[5], false, true));
        spriteScanlineBuffer[7].PixelColor.Should().BeNull();

        // Advance 15 scanlines to scanline 16: the first row of the upper pattern should be rendered
        evaluator.EvaluateSprites(16);
        spriteScanlineBuffer[7].Should().Be(new SpriteEvaluator.SpritePixelData(Ppu.Palette[5], false, true));
        spriteScanlineBuffer[0].PixelColor.Should().BeNull();
    }

    [Fact]
    public void ShouldNotEvaluateSpritesOnTheFirstScanline()
    {
        var registers = new PPURegisters { Mask = PPUMaskFlagPositions.SHOW_SPRITES };
        var memory = new Mock<IPPUMemory>();
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) => i switch
            {
                0 => new OamEntry(0, 0, 0, 10, i),
                1 => new OamEntry(0, 0, 0, 10, i),
                _ => new OamEntry(0xEF, 0, 0, 10, i)
            });

        evaluator.EvaluateSprites(0); // scanline 0: no sprite pixels are rendered

        evaluator.SpriteScanlineBuffer.Should().OnlyContain(c => c.PixelColor == null);
    }

    [Fact]
    public void Should_OnlyHave8SpritesPerScanline()
    {
        var registers = new PPURegisters { Mask = PPUMaskFlagPositions.SHOW_SPRITES };
        var memory = new Mock<IPPUMemory>();
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        // 10 sprites on the scanline -> only the first 8 are evaluated, the 9th sets overflow
        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) => i is >= 0 and < 10 ? new OamEntry(0, 0, 0, 10, i) : new OamEntry(0xEF, 0, 0, 10, i));

        evaluator.EvaluateSprites(1);

        registers.GetOverflowFlag().Should().BeTrue();
    }

    [Fact]
    public void ShouldNotRenderSpritesBeyondTheEvaluationLimit()
    {
        // 10 sprites share the scanline and the first eight of them are the ones kept for rendering.
        // They all sit on x = 0, so the eight survivors collapse into the single pixel the first sprite
        // claimed, and the 9th and 10th are dropped - which is why the overflow flag is asserted too
        var registers = new PPURegisters
        {
            Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000, matching the pattern
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        SetUpSprites(memory, [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]); // ten of them share the scanline
        SetUpSpritePattern(memory);

        var evaluator = new SpriteEvaluator(registers, memory.Object);

        evaluator.EvaluateSprites(1);

        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;

        spriteScanlineBuffer.Where(c => c.PixelColor != null).Should().HaveCount(1);
        spriteScanlineBuffer[0].Should().Be(new SpriteEvaluator.SpritePixelData(Ppu.Palette[0], false, true));

        spriteScanlineBuffer[1].PixelColor.Should().BeNull(); // nothing is drawn next to the sprite
        registers.GetOverflowFlag().Should().BeTrue(); // the 9th sprite of the scanline is reported
    }

    [Fact]
    public void SpriteOverflowFlagStaysUntilNextEvaluation()
    {
        var registers = new PPURegisters { Mask = PPUMaskFlagPositions.SHOW_SPRITES };
        var memory = new Mock<IPPUMemory>();
        var evaluator = new SpriteEvaluator(registers, memory.Object);

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) => i is >= 0 and < 10 ? new OamEntry(0, 0, 0, 10, i) : new OamEntry(0xEF, 0, 0, 10, i));

        evaluator.EvaluateSprites(1);
        registers.GetOverflowFlag().Should().BeTrue();

        evaluator.EvaluateSprites(2); // does not reset on the next scanline
        registers.GetOverflowFlag().Should().BeTrue();
    }

    private const uint CyclesPerScanline = 341;
    private const uint CyclesPerFrame = CyclesPerScanline * 262;

    // Sprites on the given rows and columns, all of them on sprite palette 1; every other OAM slot is
    // parked outside of the visible area
    private static void SetUpSprites(Mock<IPPUMemory> memory, int[] spriteRows, int[]? spriteColumns = null)
    {
        var attributes = new OamEntry.SpriteAttribute(1, BehindBackground: false, VerticalFlip: false, HorizontalFlip: false).ToByte();

        memory.Setup(m => m.GetOamEntry(It.IsAny<int>()))
            .Returns((int i) => i >= 0 && i < spriteRows.Length
                ? new OamEntry((byte)spriteRows[i], 0, attributes, (byte)(spriteColumns?[i] ?? 0), i)
                : new OamEntry(0xEF, 0, attributes, 0, i));
    }
    
    private static void SetUpSpritePattern(Mock<IPPUMemory> memory)
    {
        memory.Setup(m => m.Read(It.IsAny<ushort>()))
            .Returns((ushort address) => address switch
            {
                >= 0x1000 and <= 0x100F => 0b10000000,
                0x3F14 => 11,
                _ => 0
            });
    }

    private static int RenderedSpriteCount(SpriteEvaluator.SpritePixelData[] scanlineBuffer) =>
        scanlineBuffer.Count(pixelData => pixelData.PixelColor != null);

    [Fact]
    public void ShouldNotEvaluateSpritesOnScanlineZero()
    {
        var registers = new PPURegisters
        {
            Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        SetUpSprites(memory, [0], [0]); // a sprite covering row 1 would be selected on any later scanline
        SetUpSpritePattern(memory);

        var evaluator = new SpriteEvaluator(registers, memory.Object);

        evaluator.EvaluateSprites(0);

        // The first scanline of the frame never shows sprites: the evaluation selects none of them
        RenderedSpriteCount(evaluator.SpriteScanlineBuffer).Should().Be(0);
    }

    [Fact]
    public void ShouldEvaluateSpritesStartingOnTheSecondScanline()
    {
        var registers = new PPURegisters
        {
            Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        SetUpSprites(memory, [0]); // visible from scanline 1 on
        SetUpSpritePattern(memory);

        var evaluator = new SpriteEvaluator(registers, memory.Object);

        evaluator.EvaluateSprites(1);

        // The sprite that used to be found in the secondary OAM shows up as a rendered pixel
        var spriteScanlineBuffer = evaluator.SpriteScanlineBuffer;
        RenderedSpriteCount(spriteScanlineBuffer).Should().Be(1);
        spriteScanlineBuffer[0].Should().Be(new SpriteEvaluator.SpritePixelData(Ppu.Palette[0], false, true));
    }

    [Fact]
    public void ShouldEvaluateEverySpriteThatIsVisibleOnTheScanline()
    {
        var registers = new PPURegisters
        {
            Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        SetUpSprites(memory, [49, 45], [0, 16]); // rows 50 and 46, on separate columns so both paint
        SetUpSpritePattern(memory);

        var evaluator = new SpriteEvaluator(registers, memory.Object);

        evaluator.EvaluateSprites(46); // scanline 46: only the sprite of row 45 is on it

        RenderedSpriteCount(evaluator.SpriteScanlineBuffer).Should().Be(1);

        evaluator.EvaluateSprites(50); // scanline 50: both sprites share the scanline

        RenderedSpriteCount(evaluator.SpriteScanlineBuffer).Should().Be(2);
    }

    [Fact]
    public void ShouldNotEvaluateSpritesThatAreNotOnTheScanline()
    {
        var registers = new PPURegisters
        {
            Control = PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS, // 0x1000
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        SetUpSprites(memory, [0, 0]); // both sprites cover rows 1-8 only
        SetUpSpritePattern(memory);

        var evaluator = new SpriteEvaluator(registers, memory.Object);

        evaluator.EvaluateSprites(50); // long past the rows the sprites cover

        RenderedSpriteCount(evaluator.SpriteScanlineBuffer).Should().Be(0);
    }

    [Fact]
    public void ShouldResetSpriteOverflowOnTheNextFrame()
    {
        var registers = new PPURegisters
        {
            Control = PPUControlFlagPositions.NMI, // the overflow reset runs during the vblank interrupt
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        SetUpSprites(memory, [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]); // ten of them share the scanline
        SetUpSpritePattern(memory);

        var ppu = new Ppu(registers, memory.Object, _logger);

        ppu.Run(CyclesPerScanline * 2); // the evaluation of scanline 1 overflows

        (ppu.Read(0x2002) & PPUStatusFlagPositions.SPRITEOVERFLOW).Should().NotBe(0);

        // The sprites are gone from the scanlines of the next frame, so nothing sets the bit again
        memory.Setup(m => m.GetOamEntry(It.IsAny<int>())).Returns((int i) => new OamEntry(0xEF, 0, 0, 0, i));

        ppu.Run(CyclesPerFrame); // across the vblank and the pre-render line that clears the flag

        registers.GetOverflowFlag().Should().BeFalse();
    }

    [Fact]
    public void ReportsSpriteOverflowThroughTheStatusRegister()
    {
        var registers = new PPURegisters
        {
            Mask = PPUMaskFlagPositions.SHOW_SPRITES
        };
        var memory = new Mock<IPPUMemory>();
        SetUpSprites(memory, [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]); // ten of them share the scanline
        SetUpSpritePattern(memory);

        var ppu = new Ppu(registers, memory.Object, _logger);

        ppu.Run(CyclesPerScanline * 2); // the evaluation of scanline 1 overflows
        
        (ppu.Read(0x2002) & PPUStatusFlagPositions.SPRITEOVERFLOW).Should().NotBe(0);
    }
}
