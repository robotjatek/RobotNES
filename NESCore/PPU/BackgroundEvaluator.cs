namespace NESCore.PPU;

public class BackgroundEvaluator(LoopyRegisters loopyRegisters, IPPUMemory ppuMemory, IPPURegisters registers)
{
    private byte _ntId;
    private byte _attributeId;
    private byte _patternLow;
    private byte _patternHigh;
    
    private ushort _shiftRegisterHi;
    private ushort _shiftRegisterLo;
    private ushort _attributeRegisterLo;
    private ushort _attributeRegisterHi;
    
    // The shift registers hold the next 16 pixels, so fine x indexes them from the top down
    private int CurrentPixelBitPosition => 15 - loopyRegisters.FineX;

    public (bool opaque, Ppu.Color pixel) NextPixel(int scanlineCycles)
    {
        var paletteAddress = GetCurrentPixelPaletteAddress();
        var paletteId = ppuMemory.Read(paletteAddress) & 0x3F;
        
        var showBg = registers.ShowBackground() &&
                     (scanlineCycles >= 8 || registers.ShowBackgroundInLeftColumn());
        
        var bgPx = showBg
            ? Ppu.Palette[paletteId]
            : Ppu.Palette[ppuMemory.Read(0x3F00) & 0x3F]; // Render the default background color
                    
        return (showBg && GetBgColorPaletteId() != 0, bgPx);
    }

    public void Tick(int scanlineCycles)
    {
        MoveShiftRegisters();
    
        var phase = scanlineCycles % 8;
        if (phase == 1)
        {
            var ntAddress = loopyRegisters.GetNametableAddress();
            _ntId = ppuMemory.Read(ntAddress);
        }
        else if (phase == 3)
        {
            var atAddress = loopyRegisters.GetAttributeAddress();
            _attributeId = ppuMemory.Read(atAddress);
        }
        else if (phase == 5)
        {
            // pattern table low
            // address is based on the control register
            var patternBaseAddress = registers.GetbackgroundPatternTableAddress();
            var fineY = (ushort)loopyRegisters.GetFineYScroll();
            var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16);
            _patternLow = ppuMemory.Read(ptAddress);
        }
        else if (phase == 7)
        {
            // pattern table low
            // address is based on the control register
            var patternBaseAddress = registers.GetbackgroundPatternTableAddress();
            var fineY = (ushort)loopyRegisters.GetFineYScroll();
            var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16 + 8);
            _patternHigh = ppuMemory.Read(ptAddress);

            // Reload shift registers (lower 8 bits)
            _shiftRegisterLo = (ushort)((_shiftRegisterLo & 0xFF00) | _patternLow);
            _shiftRegisterHi = (ushort)((_shiftRegisterHi & 0xFF00) | _patternHigh);
                        
            var shiftAmount = loopyRegisters.GetAttributeShift();

            var paletteBits = (_attributeId >> shiftAmount) & 0x03;
            var attrLo = (byte)((paletteBits & 1) != 0 ? 0xFF : 0x00);
            var attrHi = (byte)(((paletteBits >> 1) & 1) != 0 ? 0xFF : 0x00);
            _attributeRegisterLo = (ushort)((_attributeRegisterLo & 0xFF00) | attrLo);
            _attributeRegisterHi = (ushort)((_attributeRegisterHi & 0xFF00) | attrHi);

            loopyRegisters.IncrementCoarseX();
        }
        
        if (scanlineCycles == 255)
        {
            loopyRegisters.IncrementY();
        }
    }
    
    private void MoveShiftRegisters()
    {
        _shiftRegisterLo <<= 1;
        _shiftRegisterHi <<= 1;

        _attributeRegisterLo <<= 1;
        _attributeRegisterHi <<= 1;
    }
    
    private ushort GetCurrentPixelPaletteAddress()
    {
        var bitPosition = CurrentPixelBitPosition;
        var colorIndex = GetBgColorPaletteId();

        if (colorIndex == 0)
        {
            return 0x3F00;
        }

        var paletteId = ((_attributeRegisterLo >> bitPosition) & 0x01) |
                        (((_attributeRegisterHi >> bitPosition) & 0x01) << 1);

        return (ushort)(0x3F00 | (paletteId << 2) | colorIndex);
    }
    
    private ushort GetBgColorPaletteId()
    {
        var bitPosition = CurrentPixelBitPosition;

        var colorIndex = ((_shiftRegisterLo >> bitPosition) & 0x01) |
                         (((_shiftRegisterHi >> bitPosition) & 0x01) << 1);

        return (ushort)(colorIndex);
    }
}