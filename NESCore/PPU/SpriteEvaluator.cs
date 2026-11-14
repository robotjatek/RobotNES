namespace NESCore.PPU;

public class SpriteEvaluator(IPPURegisters registers, IPPUMemory ppuMemory)
{
    public record struct SpritePixelData(Ppu.Color? PixelColor, bool BehindBackground, bool Zero = false);

    private readonly List<OamEntry> _secondaryOam = new(capacity: 8);

    public SpritePixelData[] SpriteScanlineBuffer { get; } = new SpritePixelData[256];

    public void EvaluateSprites(int scanline)
    {
        // TODO: make it cycle correct: https://www.nesdev.org/wiki/PPU_sprite_evaluation#Details
        // TODO: other quirks like the internal n[0-63] (6 bits), m[0-3] (2 bits) variables
        Array.Clear(SpriteScanlineBuffer, 0, SpriteScanlineBuffer.Length);
        _secondaryOam.Clear();

        var spriteSize = registers.GetSpriteSize();
        var height = spriteSize == SpriteSize._8x8 ? 8 : 16;

        var startAddress = (int)registers.OAMAddress;
        var spriteIndex = 0;

        for (var addr = startAddress; addr <= 252; addr += 4)
        {
            var sprite = startAddress == 0
                ? ppuMemory.GetOamEntry(spriteIndex)
                : ppuMemory.GetOamEntryAtAddress((byte)addr, spriteIndex);
            spriteIndex++;

            var top = sprite.ScreenY;
            var bottom = top + height;

            if (scanline < top || scanline >= bottom || top > 240)
                continue; // Skip sprites that are not visible on the current scanline

            if (_secondaryOam.Count < 8)
            {
                _secondaryOam.Add(sprite);
            }
            else
            {
                // According to the specification, the bit stays on until the next vblank, does not clear on the end of the scanline
                registers.SetOverflowFlag(true);
                break;
            }

            // Select the correct row from the pattern table
            var row = sprite.VerticalFlip ? height - 1 - (scanline - sprite.ScreenY) : scanline - sprite.ScreenY;
            var address = sprite.GetPatternTableAddress(registers.GetSpriteSize(),
                registers.GetSpritePatternTableAddress(), row);

            var patternPlaneLow = ppuMemory.Read(address);
            var patternPlaneHigh = ppuMemory.Read((ushort)(address + 8));
            var attribute = sprite.PaletteIndex;

            for (var j = 0; j < 8; j++)
            {
                var bitPos = sprite.HorizontalFlip ? j : 7 - j; // Reverse the bit walking order based on h-flip
                var lowBit = (patternPlaneLow >> bitPos) & 1;
                var highBit = (patternPlaneHigh >> bitPos) & 1;
                var pixelColor = (highBit << 1) | lowBit;
                if (pixelColor == 0 || sprite.X + j >= 256 || SpriteScanlineBuffer[sprite.X + j].PixelColor != null)
                {
                    // Both pattern bits are 0 => this pixel is transparent => skip color 0
                    // or skip when the pixel is already taken
                    continue;
                }

                var paletteIndex = (attribute << 2) | pixelColor;
                var paletteColor =
                    ppuMemory.Read((ushort)(0x3F10 + paletteIndex)); // Sprite palette is fixed to 0x3F10
                var spriteData = new SpritePixelData(Ppu.Palette[paletteColor], sprite.BehindBackground,
                    sprite.IsSpriteZero);

                if (sprite.ScreenX + j < 256)
                    SpriteScanlineBuffer[sprite.X + j] = spriteData;
            }
        }
    }
}