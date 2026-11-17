namespace NESCore.APU;

/// <summary>
/// Pulse is a square-wave channel
/// It has a timer (11 bits) and a pre-determined set of possible duty-cycles:
/// 0: 12.5%, 1: 25.0%, 2: 50.0%, 3: 75.0%
/// Every tick it decrements its internal timer. It is returning the currently set duty value until the timer is >0 then increments the dutyStep
/// The smaller the time, the faster we step to the next value => the higher the pitch
/// </summary>
public class PulseChannel
{
    private static readonly byte[][] DutyTable =
    [
        // 12.5%
        [0, 1, 0, 0, 0, 0, 0, 0],
        // 25%
        [0, 1, 1, 0, 0, 0, 0, 0],
        // 50%
        [0, 1, 1, 1, 1, 0, 0, 0],
        // 75%
        [1, 0, 0, 1, 1, 1, 1, 1]
    ];

    private byte _mode;
    private byte _volume;
    private ushort _timerPeriod;
    private ushort _timerValue;
    private byte _dutyStep;
    
    // "Once loaded with a value, it can optionally count down (when the length counter halt flag is clear). Once it reaches zero, the corresponding channel is silenced." https://www.nesdev.org/wiki/APU_Length_Counter
    public byte LengthCounter { get; private set; } // clocked by the frame counter
    private bool _lengthHalt;
    private static readonly byte[] LengthCounterTable = [10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14, 12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30]; // from nesdev wiki

    private bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (!_enabled)
            {
                LengthCounter = 0;
            }
        }
    }

    public void Write(ushort register, byte value)
    {
        switch (register)
        {
            case 0:
                // Pulse settings: DDLC VVVV 
                // D: pulse width: 0: 12.5%, 1: 25%, 2: 50%, 3: 75%
                // L: Length counter, halt flag, envelope loop flag
                // C: Constant volume flag (override envelope with V)
                // V: volume
                _mode = (byte)((value >> 6) & 0x03);
                _volume = (byte)(value & 0x0f);
                _lengthHalt = (value & 0b00100000) > 0;
                break;
            case 1:
                // Sweep
                break;
            case 2:
                // timer low: TTTT TTTT
                _timerPeriod = (ushort)((_timerPeriod & 0x0700) | value);
                break;
            case 3:
                // LLLL LTTT
                // Length counter load (L), Timer high (T), Restart envelope and pulse phase.
                _timerPeriod = (ushort)((_timerPeriod & 0x00FF) | (value & 0b111) << 8);
                if (Enabled)
                    LengthCounter = LengthCounterTable[(value >> 3) & 0b11111]; // only can be set when enabled
                _dutyStep = 0; // restart phase
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(register));
        }
    }
    
    public void Tick()
    {
        if (!Enabled)
        {
            LengthCounter = 0; // forced to zero until it is enabled
        }
        
        if (_timerValue == 0)
        {
            _timerValue = _timerPeriod;
            _dutyStep = (byte)((_dutyStep + 1) & 0x07);
        }
        else
        {
            _timerValue--;
        }
    }

    public void TickLenghtCounter()
    {
        if (LengthCounter == 0 || _lengthHalt)
        {
            return;
        }
        
        LengthCounter--;
    }

    public byte GetOutput()
    {
        if (!Enabled || _timerPeriod < 8 || LengthCounter == 0)
            return 0;
        
        return DutyTable[_mode][_dutyStep] == 1 ? _volume : (byte)0;
    }
}