namespace NESCore.Mappers.MMC1;

// TODO: ignore Consecutive-cycle writes (Bill and Ted fails without it) => maybe inside the mapper
//  bit7 reset is NOT ignored only data writes!
/// <summary>
/// 5-bit shift register for MMC1. On the 5th write it fires its action
/// </summary>
/// <param name="action"></param>
public class ShiftRegister(Action<ushort, byte> action, Action resetAction)
{
    private int _remainingShift = 5;
    private byte _register; // 5 bit -- note: the real hardware initializes D5 to 1 to be able to detect when the 5th write happens. I have an independent counter so this is not needed here
    private uint _lastWriteCycles;

    /// <summary>
    /// Write the LSB to the top of the register => LSB first output
    /// </summary>
    /// <param name="value"></param>
    /// <param name="destination"></param>
    /// <param name="cycles">The cycle count of the bus when the write happened</param>
    public void Write(byte value, ushort destination, uint cycles)
    {
        // if bit 7 is set => reset
        if ((value & 0x80) > 0)
        {
            resetAction();
            _remainingShift = 5;
            _register = 0;
            return;
        }
        
        // ignore write if it happened one cycle after the previous
        if (cycles == _lastWriteCycles + 1)
        {
            return;
        }
        _lastWriteCycles = cycles;
        
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