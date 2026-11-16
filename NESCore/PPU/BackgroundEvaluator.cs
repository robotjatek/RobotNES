namespace NESCore.PPU;

public class BackgroundEvaluator
{
    private readonly LoopyRegisters _loopyRegisters;
    private readonly IPPUMemory _ppuMemory;
    private readonly IPPURegisters _registers;
    
    private byte _ntId;
    private byte _attributeId;
    private byte _patternLow;
    private byte _patternHigh;
    
    private ushort _shiftRegisterHi;
    private ushort _shiftRegisterLo;
    private ushort _attributeRegisterLo;
    private ushort _attributeRegisterHi;
    
    // The shift registers hold the next 16 pixels, so fine x indexes them from the top down
    private int CurrentPixelBitPosition => 15 - _loopyRegisters.FineX;

    public BackgroundEvaluator(LoopyRegisters loopyRegisters, IPPUMemory ppuMemory, IPPURegisters registers)
    {
        _loopyRegisters = loopyRegisters;
        _ppuMemory = ppuMemory;
        _registers = registers;
    }
    
    public (bool opaque, Ppu.Color pixel) NextPixel(int scanlineCycles)
    {
        var paletteAddress = GetCurrentPixelPaletteAddress();
        var paletteId = _ppuMemory.Read(paletteAddress) & 0x3F;
        
        var showBg = _registers.ShowBackground() &&
                     (scanlineCycles >= 8 || _registers.ShowBackgroundInLeftColumn());
        
        var bgPx = showBg
            ? Ppu.Palette[paletteId]
            : Ppu.Palette[_ppuMemory.Read(0x3F00)]; // Render the default background color
                    
        return (showBg && GetBgColorPaletteId() != 0, bgPx);
    }

    public void Tick(int scanlineCycles)
    {
        MoveShiftRegisters();
    
        var phase = scanlineCycles % 8;
        if (phase == 1)
        {
            var ntAddress = _loopyRegisters.GetNametableAddress();
            _ntId = _ppuMemory.Read(ntAddress);
        }
        else if (phase == 3)
        {
            var atAddress = _loopyRegisters.GetAttributeAddress();
            _attributeId = _ppuMemory.Read(atAddress);
        }
        else if (phase == 5)
        {
            // pattern table low
            // address is based on the control register
            var patternBaseAddress = _registers.GetbackgroundPatternTableAddress();
            var fineY = (ushort)_loopyRegisters.GetFineYScroll();
            var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16);
            _patternLow = _ppuMemory.Read(ptAddress);
        }
        else if (phase == 7)
        {
            // pattern table low
            // address is based on the control register
            var patternBaseAddress = _registers.GetbackgroundPatternTableAddress();
            var fineY = (ushort)_loopyRegisters.GetFineYScroll();
            var ptAddress = (ushort)(patternBaseAddress + fineY + _ntId * 16 + 8);
            _patternHigh = _ppuMemory.Read(ptAddress);

            // Reload shift registers (lower 8 bits)
            _shiftRegisterLo = (ushort)((_shiftRegisterLo & 0xFF00) | _patternLow);
            _shiftRegisterHi = (ushort)((_shiftRegisterHi & 0xFF00) | _patternHigh);
                        
            var shiftAmount = _loopyRegisters.GetAttributeShift();

            var paletteBits = (_attributeId >> shiftAmount) & 0x03;
            var attrLo = (byte)((paletteBits & 1) != 0 ? 0xFF : 0x00);
            var attrHi = (byte)(((paletteBits >> 1) & 1) != 0 ? 0xFF : 0x00);
            _attributeRegisterLo = (ushort)((_attributeRegisterLo & 0xFF00) | attrLo);
            _attributeRegisterHi = (ushort)((_attributeRegisterHi & 0xFF00) | attrHi);

            _loopyRegisters.IncrementCoarseX();
        }
        
        if (scanlineCycles == 255)
        {
            _loopyRegisters.IncrementY();
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

        var paletteId = (((_attributeRegisterLo >> bitPosition) & 0x01) |
                         (((_attributeRegisterHi >> bitPosition) & 0x01) << 1));

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