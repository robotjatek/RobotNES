namespace NESCore.PPU;

public class LoopyRegisters
{
    // yyy: fine Y scroll, NN: nametable, YYYYY: coarse Y scroll, XXXXX: coarse x scroll
    public ushort V { get; private set; } // format: 0 yyy NN YYYYY XXXXX 
    public ushort T { get; private set; }
    public byte FineX { get; private set; }
    private bool _latch;

    private const ushort CoarseYMask = 0x03E0;
    private const ushort CoarseYOffset = 5;
    private const ushort FineYMask = 0x7000;
    private const ushort FineYOffset = 12;
    private const ushort CoarseXMask = 0x01F;
    private const ushort PaletteStart = 0x3F00;
    private const ushort PaletteMirrorOffset = 0x1000;

    public void WriteControl(byte control)
    {
        // Put the last 2 bits into the bits 10 and 11 of t
        T = (ushort)((T & 0xF3FF) | (control & 0b11) << 10);
    }

    public void ReadStatus()
    {
        _latch = false;
    }

    public byte GetFineYScroll()
    {
        return (byte)((V & FineYMask) >> FineYOffset);
    }

    // True when v points into palette memory ($3F00-$3FFF), where reads bypass the data buffer
    public bool IsPaletteAccess()
    {
        return V >= PaletteStart;
    }
    
    public ushort GetPaletteMirrorAddress()
    {
        return (ushort)(V - PaletteMirrorOffset);
    }

    // Bit offset of the 2-bit group in the attribute byte that holds the current tile's palette.
    // Coarse x bit1 and coarse y bit1 select the quadrant, each worth two bits within the byte.
    public int GetAttributeShift()
    {
        return (V & 0x0002) | ((V & 0x0040) >> 4);
    }

    public void WriteAddress(byte value)
    {
        if (!_latch)
        {
            T = (ushort)((T & 0x00ff) | ((value & 0x3f) << 8));
        }
        else
        {
            T = (ushort)((T & 0xff00) | value);
            V = T;
        }

        _latch = !_latch;
    }

    public void IncrementAddress(IPPURegisters registers)
    {
        V = (ushort)((V + registers.VRAMIncrement()) & 0x3fff);
    }

    public void ScrollWrite(byte value)
    {
        if (!_latch)
        {
            FineX = (byte)(value & 0b111);
            T = (ushort)((T & 0xFFE0) | (value >> 3));
        }
        else
        {
            SetYScroll(value);
        }

        _latch = !_latch;
    }

    private void SetYScroll(byte value)
    {
        var fineY = value & 0b111;
        var coarseY = value >> 3;

        T = (ushort)((T & 0x8C1F) // mask out fineY and coarseY
                     | (fineY << 12)
                     | (coarseY << 5));
    }

    public void IncrementCoarseX()
    {
        // From https://www.nesdev.org/wiki/PPU_scrolling
        // wrap around:
        if ((V & CoarseXMask) == CoarseXMask)
        {
            V &= 0xFFE0; // zero out the bottom 5 bits (coarse x part of the address)
            V ^= 0x0400; // Switch to next nametable
        }
        else
        {
            V = (ushort)(V + 1);
        }
    }

    public void IncrementY()
    {
        // increment fineY at the end of the scanline
        var fineY = GetFineYScroll();
        if (fineY < 7)
        {
            V += 0x1000; // increment fineY
        }
        else
        {
            var coarseY = (ushort)((V & CoarseYMask) >> CoarseYOffset);
            V &= 0x8FFF;

            if (coarseY == 29)
            {
                // end of the nametable => change NT
                // coarseY = 0;
                // NT++
                coarseY = 0;
                V ^= 0x0800;
            }
            else if (coarseY == 31)
            {
                coarseY = 0;
            }
            else
            {
                coarseY++;
            }

            V = (ushort)((V & ~CoarseYMask) | (coarseY << CoarseYOffset));
        }
    }

    // Reset horizontal scroll (coarse X and NT horizontal bit)
    public void ResetHorizontalScroll()
    {
        V = (ushort)((V & ~0x041F) | (T & 0x041F));
    }

    public void ResetVerticalScroll()
    {
        V = (ushort)((V & ~0x7BE0) | (T & 0x7BE0));
    }

    public ushort GetNametableAddress()
    {
        // nametable base is 0x2000, the low 12 bits of v select table, tile row and tile column
        return (ushort)(0x2000 | (V & 0x0FFF));
    }

    public ushort GetAttributeAddress()
    {
        // Fetch attribute table
        // attribute address = 0x23C0 | (v & 0x0C00) | ((v >> 4) & 0x38) | ((v >> 2) & 0x07)
        return (ushort)(0x23C0 | (V & 0x0C00) | ((V >> 4) & 0x0038) |
                        ((V >> 2) & 0x0007));
    }
}
