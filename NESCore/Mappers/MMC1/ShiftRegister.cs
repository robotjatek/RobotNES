namespace NESCore.Mappers.MMC1;

/// <summary>
/// 5-bit shift register for MMC1. On the 5th write it fires its action
/// </summary>
/// <param name="action"></param>
public class ShiftRegister(Action<ushort, byte> action, Action resetAction)
{
    private int _remainingShift = 5;
    private byte _register; // 5 bit -- note: the real hardware initializes D5 to 1 to be able to detect when the 5th write happens. I have an independent counter so this is not needed here
    private uint _lastCycle;

    /// <summary>
    /// Write the LSB to the top of the register => LSB first output
    /// </summary>
    /// <param name="value"></param>
    /// <param name="destination"></param>
    /// <param name="cycles">The bus cycle counter at this write. The bus advances it once per bus
    /// cycle (any access, read or write), so two writes one bus cycle apart are the two phases of one
    /// read-modify-write: the original value written back, then the modified value.
    /// nesdev: "When the serial port is written to on consecutive cycles, it ignores every write
    /// after the first." Bit 7 is checked BEFORE this filter, so the reset is never ignored — Shinsenden
    /// relies on that via the illegal RRA abs,X.</param>
    public void Write(byte value, ushort destination, uint cycles)
    {
        var consecutiveCycle = cycles == _lastCycle + 1;
        _lastCycle = cycles;

        // if bit 7 is set => reset
        if ((value & 0x80) > 0)
        {
            resetAction();
            _remainingShift = 5;
            _register = 0;
            return;
        }

        if (consecutiveCycle)
        {
            return;
        }
        
        _register >>= 1;
        _register |= (byte)((value & 1) << 4); // shift the LSB to the 5th bit
        _remainingShift--;
        
        if (_remainingShift == 0)
        {
            action(destination, (byte)(_register & 0x1f));
            _register = 0;
            _remainingShift = 5;
        }
    }
}