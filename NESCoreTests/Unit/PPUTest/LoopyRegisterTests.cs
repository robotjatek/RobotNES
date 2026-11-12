using Moq;
using NESCore.PPU;

namespace NESCoreTests.Unit.PPUTest;

public class LoopyRegisterTests : TestBase
{
    private static LoopyRegisters CreateLoopy() => new();

    private Ppu CreatePpuWithRenderingEnabled(byte control = 0x00)
    {
        var registers = new PPURegisters
        {
            Control = control,
            Mask = 0x8 // Show background
        };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0x00);

        return new Ppu(registers, memory.Object, _logger);
    }

    [Fact]
    public void WriteControlShouldSetTheNametableBitsInTemp()
    {
        var loopy = CreateLoopy();

        loopy.WriteControl(0b0000_0011);

        ScrollBits.Nametable(loopy.T).Should().Be(0b11);
    }

    [Fact]
    public void WriteControlShouldNotTouchTheScrollBits()
    {
        var loopy = CreateLoopy();
        loopy.ScrollWrite(0x7D); // coarse X = 0x0F, fine X = 0x05

        loopy.WriteControl(0b0000_0011);

        ScrollBits.Nametable(loopy.T).Should().Be(0b11);
        ScrollBits.CoarseX(loopy.T).Should().Be(0x0F, "coarse X survives");
        loopy.FineX.Should().Be(0x05);
    }

    [Fact]
    public void FirstScrollWriteShouldSetTheHorizontalScroll()
    {
        var loopy = CreateLoopy();

        // 0x7D = 0b000_11111_101 => coarse X = 0b11111, fine X = 0b101
        loopy.ScrollWrite(0x7D);

        loopy.FineX.Should().Be(0x05);
        ScrollBits.CoarseX(loopy.T).Should().Be(0x0F);
        ScrollBits.VerticalHalf(loopy.T).Should().Be(0x0000, "the horizontal byte must not reach the vertical half");
    }

    [Fact]
    public void FirstScrollWriteShouldUseTheWholeCoarseXRange()
    {
        var loopy = CreateLoopy();

        // 0xFA = 0b000_11111_010 => the highest coarse X scroll must stay reachable
        loopy.ScrollWrite(0xFA);

        loopy.FineX.Should().Be(0x02);
        ScrollBits.CoarseX(loopy.T).Should().Be(0x1F);
    }

    [Fact]
    public void SecondScrollWriteShouldSetTheVerticalScroll()
    {
        var loopy = CreateLoopy();

        loopy.ScrollWrite(0x7D);
        loopy.ScrollWrite(0b1010_1010);

        ScrollBits.FineY(loopy.T).Should().Be(0b010);
        ScrollBits.CoarseY(loopy.T).Should().Be(0b10101);
    }

    [Fact]
    public void ScrollWritesAlternateBetweenTheHalvesOfTempByTheirCount()
    {
        var loopy = CreateLoopy();

        // 1st write: horizontal half only
        loopy.ScrollWrite(0x0F); // fine X = 7, coarse X = 1
        loopy.FineX.Should().Be(0x07);
        ScrollBits.CoarseX(loopy.T).Should().Be(0x01);
        ScrollBits.VerticalHalf(loopy.T).Should().Be(0x0000);

        // 2nd write: vertical half only
        loopy.ScrollWrite(0xF0); // fine Y = 0, coarse Y = 30
        ScrollBits.FineY(loopy.T).Should().Be(0x00);
        ScrollBits.CoarseY(loopy.T).Should().Be(0x1E);

        // 3rd write: wraps back to the horizontal half
        loopy.ScrollWrite(0x1F); // fine X = 7, coarse X = 3
        loopy.FineX.Should().Be(0x07);
        ScrollBits.CoarseX(loopy.T).Should().Be(0x03);
        ScrollBits.CoarseY(loopy.T).Should().Be(0x1E, "the Y scroll set by the 2nd write survives");
    }

    [Fact]
    public void SecondScrollWriteShouldNotClobberTheHorizontalScroll()
    {
        var loopy = CreateLoopy();

        loopy.ScrollWrite(0x7D);
        loopy.ScrollWrite(0xAA);

        loopy.FineX.Should().Be(0x05);
        ScrollBits.CoarseX(loopy.T).Should().Be(0x0F);
    }

    [Fact]
    public void ScrollWriteShouldNotClobberTheNametableBits()
    {
        var loopy = CreateLoopy();

        loopy.WriteControl(0b0000_0011);
        loopy.ScrollWrite(0xFA);
        loopy.ScrollWrite(0xFE);

        ScrollBits.Nametable(loopy.T).Should().Be(0b11);
    }

    [Fact]
    public void FirstAddressWriteShouldOnlyTouchTheHighBitsOfTemp()
    {
        var loopy = CreateLoopy();

        loopy.WriteAddress(0x22);

        loopy.T.Should().Be(0x2200);
        loopy.V.Should().Be(0x0000, "v is only loaded once the second byte arrives");
    }

    [Fact]
    public void SecondAddressWriteShouldCopyTempIntoVertical()
    {
        var loopy = CreateLoopy();

        loopy.WriteAddress(0x22);
        loopy.WriteAddress(0x11);

        loopy.T.Should().Be(0x2211);
        loopy.V.Should().Be(0x2211);
    }

    [Fact]
    public void ReadStatusShouldStartANewAddressPair()
    {
        var loopy = CreateLoopy();

        // Half a pair: t carries the high byte, v still waits for the low one.
        loopy.WriteAddress(0x22);

        loopy.ReadStatus();

        // The write that used to close the pair now opens one: it lands on the high byte
        // and leaves v alone.
        loopy.WriteAddress(0x11);

        loopy.T.Should().Be(0x1100);
        loopy.V.Should().Be(0x0000);
    }

    [Fact]
    public void ReadStatusShouldNotChangeTheAddresses()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x22);
        loopy.WriteAddress(0x11);

        loopy.ReadStatus();

        loopy.V.Should().Be(0x2211);
        loopy.T.Should().Be(0x2211);
    }

    [Fact]
    public void IncrementAddressShouldAddOneByDefault()
    {
        var loopy = CreateLoopy();

        loopy.IncrementAddress(new PPURegisters { Control = 0x00 });

        loopy.V.Should().Be(0x0001);
    }

    [Fact]
    public void IncrementAddressShouldAdd32WhenVramIncrementIsEnabled()
    {
        var loopy = CreateLoopy();

        loopy.IncrementAddress(new PPURegisters { Control = 0x04 });

        loopy.V.Should().Be(0x0020);
    }

    [Fact]
    public void IncrementCoarseXShouldAdvanceTheTileColumnWithoutMovingTemp()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x20);
        loopy.WriteAddress(0x00); // coarse X = 0, left nametable
        var verticalBefore = ScrollBits.VerticalHalf(loopy.V);

        loopy.IncrementCoarseX();

        ScrollBits.CoarseX(loopy.V).Should().Be(0x01);
        ScrollBits.VerticalHalf(loopy.V).Should().Be(verticalBefore, "only the tile column moves");
        ScrollBits.CoarseX(loopy.T).Should().Be(0x00, "t has to keep the scroll set by $2005/$2000");
    }

    [Fact]
    public void IncrementCoarseXShouldWrapIntoTheRightHandNametable()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x00);
        loopy.WriteAddress(0x1F); // coarse X = 31

        loopy.IncrementCoarseX();

        ScrollBits.CoarseX(loopy.V).Should().Be(0, "the column wraps back to the left edge");
        ScrollBits.HorizontalNametable(loopy.V).Should().BeTrue();
        ScrollBits.CoarseX(loopy.T).Should().Be(31, "t keeps the scroll the game asked for");
    }

    [Fact]
    public void ResetHorizontalScrollShouldRestoreTheColumnAndNametableFromTemp()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x20);
        loopy.WriteAddress(0x00); // v = t, left nametable
        loopy.WriteControl(0b0000_0001); // right-hand nametable in t
        loopy.ScrollWrite(0x1F); // coarse X = 3, fine X = 7
        var verticalBefore = ScrollBits.VerticalHalf(loopy.V);

        loopy.IncrementCoarseX();
        ScrollBits.CoarseX(loopy.V).Should().Be(0x01);

        loopy.ResetHorizontalScroll();

        ScrollBits.CoarseX(loopy.V).Should().Be(0x03, "coarse X comes back from t");
        ScrollBits.HorizontalNametable(loopy.V).Should().BeTrue();
        ScrollBits.VerticalHalf(loopy.V).Should().Be(verticalBefore, "the vertical half is untouched");
    }

    [Fact]
    public void IncrementYShouldAdvanceToTheNextPixelRow()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x20);
        loopy.WriteAddress(0x00);
        var fineYBefore = ScrollBits.FineY(loopy.V);

        loopy.IncrementY();

        ScrollBits.FineY(loopy.V).Should().Be((byte)(fineYBefore + 1));
        ScrollBits.CoarseY(loopy.V).Should().Be(0, "still inside the same tile row");
    }

    [Fact]
    public void IncrementYShouldWrapFineYIntoCoarseY()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x30); // fine Y = 3
        loopy.WriteAddress(0x00);

        // fine Y: 3 -> 7, the fourth increment only bumps fine Y
        for (var i = 0; i < 4; i++) loopy.IncrementY();
        ScrollBits.FineY(loopy.V).Should().Be(7);

        loopy.IncrementY();

        ScrollBits.FineY(loopy.V).Should().Be(0, "the pixel row wraps");
        ScrollBits.CoarseY(loopy.V).Should().Be(1, "and the tile row advances");
    }

    [Fact]
    public void IncrementYShouldSwitchTheVerticalNametableAtTheBottomOfTheTileRows()
    {
        var loopy = CreateLoopy();
        loopy.ScrollWrite(0x00);
        loopy.ScrollWrite(0xEF); // fine Y = 7, coarse Y = 29 - the last row before the nametable flips
        loopy.ResetVerticalScroll();
        ScrollBits.VerticalNametable(loopy.V).Should().BeFalse();

        loopy.IncrementY();

        ScrollBits.CoarseY(loopy.V).Should().Be(0);
        ScrollBits.VerticalNametable(loopy.V).Should().BeTrue();
    }

    [Fact]
    public void IncrementYShouldWrapCoarseYWithoutSwitchingNametablePastTheLastTileRow()
    {
        var loopy = CreateLoopy();
        loopy.ScrollWrite(0x00);
        loopy.ScrollWrite(0xFF); // fine Y = 7, coarse Y = 31 - beyond the nametable flip
        loopy.ResetVerticalScroll();

        loopy.IncrementY();

        ScrollBits.CoarseY(loopy.V).Should().Be(0);
        ScrollBits.VerticalNametable(loopy.V).Should().BeFalse("coarse Y 31 wraps without changing nametable");
    }

    [Fact]
    public void ResetVerticalScrollShouldRestoreTheVerticalScrollFromTemp()
    {
        var loopy = CreateLoopy();
        loopy.ScrollWrite(0x00); // X scroll 0
        loopy.ScrollWrite(0x5E); // fine Y = 6, coarse Y = 11

        loopy.ResetVerticalScroll();

        ScrollBits.FineY(loopy.V).Should().Be(6);
        ScrollBits.CoarseY(loopy.V).Should().Be(11);
    }

    [Fact]
    public void GetNametableAddressShouldCombineTheBaseWithTheLowBitsOfV()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x20);
        loopy.WriteAddress(0x05);

        loopy.GetNametableAddress().Should().Be(0x2005);
    }

    [Fact]
    public void GetAttributeAddressShouldBeDerivedFromV()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x20);
        loopy.WriteAddress(0x00);

        loopy.GetAttributeAddress().Should().Be(0x23C0);
    }

    [Theory]
    [InlineData(0x20, 0x00, false)] // start of the nametables
    [InlineData(0x3E, 0xFF, false)] // just below the palette window
    [InlineData(0x3F, 0x00, true)]
    [InlineData(0x3F, 0x10, true)]
    public void IsPaletteAccessShouldFollowV(byte high, byte low, bool expected)
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(high);
        loopy.WriteAddress(low);

        loopy.IsPaletteAccess().Should().Be(expected);
    }

    [Fact]
    public void GetPaletteMirrorAddressShouldPointAtTheMirroredNametable()
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(0x3F);
        loopy.WriteAddress(0x10);

        loopy.GetPaletteMirrorAddress().Should().Be(0x2F10);
    }

    [Theory]
    [InlineData(0x20, 0x00, 0)] // top-left quadrant
    [InlineData(0x20, 0x02, 2)] // coarse x bit1
    [InlineData(0x20, 0x40, 4)] // coarse y bit1
    [InlineData(0x20, 0x42, 6)] // bottom-right quadrant
    public void GetAttributeShiftShouldSelectTheQuadrantOfTheAttributeByte(byte high, byte low, int expected)
    {
        var loopy = CreateLoopy();
        loopy.WriteAddress(high);
        loopy.WriteAddress(low);

        loopy.GetAttributeShift().Should().Be(expected);
    }

    [Fact]
    public void ShouldIncrementCoarseXOnCycle8()
    {
        var ppu = CreatePpuWithRenderingEnabled();
        ppu.Write(0x2006, 0x20);
        ppu.Write(0x2006, 0x00); // v = 0x2000 => coarse X = 0

        ppu.Run(8);

        ScrollBits.CoarseX(ppu.Loopy.V).Should().Be(0x01);
    }

    [Fact]
    public void CoarseXShouldWrapAroundOn32()
    {
        var ppu = CreatePpuWithRenderingEnabled();
        ppu.Write(0x2006, 0x00);
        ppu.Write(0x2006, 0x1F); // coarse X = 31

        ppu.Run(8);

        ScrollBits.CoarseX(ppu.Loopy.V).Should().Be(0);
        ScrollBits.HorizontalNametable(ppu.Loopy.V).Should().BeTrue();
    }

    [Fact]
    public void ShouldIncrementFineYOnCycle256()
    {
        var ppu = CreatePpuWithRenderingEnabled();
        ppu.Write(0x2006, 0x20);
        ppu.Write(0x2006, 0x00);
        var fineYBefore = ScrollBits.FineY(ppu.Loopy.V);

        ppu.Run(256);

        ScrollBits.FineY(ppu.Loopy.V).Should().Be((byte)(fineYBefore + 1),
            "cycle 255 moves down one pixel row");
    }

    [Fact]
    public void ShouldRestoreXAtTheEndOfTheScanline()
    {
        var ppu = CreatePpuWithRenderingEnabled();
        ppu.Write(0x2006, 0x20);
        ppu.Write(0x2006, 0x00); // scroll 0: coarse X = 0, left nametable
        var fineYBefore = ScrollBits.FineY(ppu.Loopy.V);

        ppu.Run(257); // cycle 256 reloads the horizontal scroll from t

        ScrollBits.CoarseX(ppu.Loopy.V).Should().Be(0, "the column restarts at the left edge");
        ScrollBits.HorizontalNametable(ppu.Loopy.V).Should().BeFalse();
        ScrollBits.FineY(ppu.Loopy.V).Should().Be((byte)(fineYBefore + 1),
            "only the horizontal half is reloaded, fine Y keeps the increment from cycle 255");
    }

    [Fact]
    public void ShouldWrapFineYAndIncrementCoarseY_WhenFineYIs7()
    {
        var ppu = CreatePpuWithRenderingEnabled();
        ppu.Write(0x2006, 0x30); // v = 0x3000 => fine Y = 3
        ppu.Write(0x2006, 0x00);

        // 4 full scanlines => + 5th scanline cycle 255
        ppu.Run(1620);

        ScrollBits.FineY(ppu.Loopy.V).Should().Be(0);
        ScrollBits.CoarseY(ppu.Loopy.V).Should().Be(1);
    }

    [Fact]
    public void ShouldIncrementNameTableBit()
    {
        var ppu = CreatePpuWithRenderingEnabled();
        // fine Y = 3, coarse Y = 29 - the last tile row, so one more wraps the nametable
        ppu.Write(0x2006, 0x33);
        ppu.Write(0x2006, 0xA0);

        ppu.Run(1620);

        ScrollBits.CoarseY(ppu.Loopy.V).Should().Be(0);
        ScrollBits.VerticalNametable(ppu.Loopy.V).Should().BeTrue();
        ScrollBits.HorizontalNametable(ppu.Loopy.V).Should()
            .BeTrue("the column wrapped past tile 31 during the scanline");
        ScrollBits.CoarseX(ppu.Loopy.V).Should().Be(2, "the two prefetch fetches past the visible area");
    }

    [Fact]
    public void ShouldCopyVerticalBitsFromTempAddress()
    {
        var ppu = CreatePpuWithRenderingEnabled();
        ppu.Write(0x2005, 0x00); // X scroll 0
        ppu.Write(0x2005, 0x5E); // Y scroll => fine Y = 6, coarse Y = 11

        ppu.Run(261 * 341 + 304); // scanline 261 (pre-render), cycle 304

        ScrollBits.FineY(ppu.Loopy.V).Should().Be(6);
        ScrollBits.CoarseY(ppu.Loopy.V).Should().Be(11);
    }

    [Fact]
    public void ScrollRegisterShouldDriveTheFetchedNametableColumn()
    {
        var registers = new PPURegisters { Mask = 0x8 };
        var memory = new Mock<IPPUMemory>();
        memory.Setup(m => m.Read(It.IsAny<ushort>())).Returns(0x00);
        var ppu = new Ppu(registers, memory.Object, _logger);

        ppu.Read(0x2002); // reset the shared latch
        ppu.Write(0x2005, 0xFA); // fine X = 2, coarse X = 31
        ppu.Write(0x2005, 0x00); // Y scroll 0

        // $2005 only feeds t, so cycle 256 is the first moment v carries the scroll.
        ppu.Run(257);

        ppu.Loopy.FineX.Should().Be(2);
        memory.Verify(m => m.Read(0x201F), Times.AtLeastOnce,
            "tile column 31 of the left nametable");
    }

    [Fact]
    public void StatusReadShouldPutTheNextScrollWriteBackOnTheHorizontalHalf()
    {
        var ppu = CreatePpuWithRenderingEnabled();

        ppu.Write(0x2005, 0x01); // X scroll
        ppu.Read(0x2002); // games read the status before reprogramming the scroll
        ppu.Write(0x2005, 0x02); // must land on the X scroll, not the Y scroll

        ppu.Loopy.FineX.Should().Be(2);
        ScrollBits.CoarseY(ppu.Loopy.T).Should().Be(0, "the byte belongs to the horizontal half");
    }
}
