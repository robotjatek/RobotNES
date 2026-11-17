namespace NESCore.APU;

/// <summary>
/// Pulse is a square-wave channel
/// It has a timer (11 bits) and a pre-determined set of possible duty-cycles:
/// 0: 12.5%, 1: 25.0%, 2: 50.0%, 3: 75.0%
/// Every tick it decrements its internal timer. It is returning the currently set duty value until the timer is >0 then increments the dutyStep
/// The smaller the time, the faster we step to the next value => the higher the pitch
/// Its output is either 0 or VOLUME (0-15), or volume scaled by the envelope. The length of a note is determined by the length counter
/// </summary>
public class PulseChannel(bool channelOne)
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
    
    // https://www.nesdev.org/wiki/APU_Envelope
    private bool _constantMode;
    private bool _envelopeStart;
    private byte _envelopeCounter;
    private byte _envelopeDivider;

    private readonly bool _channelOne = channelOne;
    private bool _sweepEnable;
    private byte _sweepPeriod;
    private bool _sweepNegate;
    private byte _sweepShift; // the amount of right shift of the timerPeriod
    private bool _sweepReload; // reloading the sweep is not instant 
    private byte _sweepDivider;
    
    // "Once loaded with a value, it can optionally count down (when the length counter halt flag is clear). Once it reaches zero, the corresponding channel is silenced." https://www.nesdev.org/wiki/APU_Length_Counter
    public byte LengthCounter { get; private set; } // clocked by the frame counter
    private bool _lengthHalt;
    private static readonly byte[] LengthCounterTable = [10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14, 12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30]; // from nesdev wiki

    public bool Enabled
    {
        get;
        set
        {
            field = value;
            if (!field)
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
                _volume = (byte)(value & 0x0f); // volume / envelope period
                _lengthHalt = (value & 0b00100000) > 0;
                _constantMode = (value & 0b00010000) > 0;
                break;
            case 1:
                // Sweep: EPPP NSSS
                _sweepEnable = (value & 0b10000000) > 0; // E: Sweep enable flag
                _sweepPeriod = (byte)((value >> 4) & 0b111); // P: Clock rate (3 bit)
                _sweepNegate = (value & 0b0000_1000) > 0; // N: Negate flag
                _sweepShift = (byte)(value & 0b111); // S: bit shift count (3 bit)
                _sweepReload = true;
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
                _envelopeStart = true; // restart the envelope on a new sound
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

    public void TickEnvelope()
    {
        if (_envelopeStart)
        {
            // "When the divider is clocked while at 0, it is loaded with V and clocks the decay level counter."
            _envelopeStart = false;
            _envelopeCounter = 15;
            _envelopeDivider = _volume;
            return;
        }
        
        if (_envelopeDivider > 0)
        {
            _envelopeDivider--;
        }
        else
        {
            _envelopeDivider = _volume;
            if (_envelopeCounter > 0)
            {
                _envelopeCounter--;
            }
            else if (_lengthHalt)
            {
                _envelopeCounter = 15;
            }
        }
    }

    public void TickSweep()
    {
        var target = CalculateSweepTarget();

        if (_sweepDivider == 0 && _sweepEnable && _sweepShift > 0 && !SweepMuting())
        {
            if (target >= 0)
            {
                _timerPeriod = (ushort)target;
            }
        }

        if (_sweepDivider == 0 || _sweepReload)
        {
            _sweepDivider = _sweepPeriod;
            _sweepReload = false;
        }
        else
        {
            _sweepDivider--;
        }
    }

    private int CalculateSweepTarget()
    {
        var change = _timerPeriod >> _sweepShift;
        if (_sweepNegate)
        {
            return _timerPeriod - change - (_channelOne ? 1 : 0);
        }

        return _timerPeriod + change;
    }

    private bool SweepMuting()
    {
        return _timerPeriod < 8 || CalculateSweepTarget() > 0x7FF;
    }

    public byte GetOutput()
    {
        if (!Enabled || SweepMuting() || LengthCounter == 0)
            return 0;

        if (DutyTable[_mode][_dutyStep] == 0)
            return 0;

        return _constantMode ? _volume : _envelopeCounter;
    }
}