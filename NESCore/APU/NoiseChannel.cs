namespace NESCore.APU;

//https://www.nesdev.org/wiki/APU_Noise
public class NoiseChannel
{

    private static readonly ushort[] PeriodTable = [4, 8, 16, 32, 64, 96, 128, 160, 202, 254, 380, 508, 762, 1016, 2034, 4068];
    private static readonly byte[] LengthCounterTable = [10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14, 12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30]; // from nesdev wiki
    
    public byte LengthCounter { get; private set; }
    private ushort _shiftRegister = 1; // 15 bits
    private byte _volume;
    private bool _constantMode;
    private bool _envelopeStart;
    private byte _envelopeCounter;
    private byte _envelopeDivider;
    private bool _halt;
    private bool _periodicNoiseMode;
    private byte _noisePeriod;

    private ushort _timerPeriod;
    private ushort _timerValue;
    
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
                _halt = (value & 0b0010_0000) > 0;
                _constantMode = (value & 0b0001_0000) > 0;
                _volume = (byte)(value & 0b0000_1111); 
                break;
            case 1:
                // unused
                break;
            case 2:
                _periodicNoiseMode = (value & 0x80) > 0; // periodic noise mode
                _timerPeriod = PeriodTable[(byte)(value & 0b1111)];
                break;
            case 3:
                if (Enabled)
                    LengthCounter = LengthCounterTable[(value >> 3) & 0b11111];
                _envelopeStart = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(register));
        }
    }

    public void Tick()
    {
        if (_timerValue == 0)
        {
            _timerValue = _timerPeriod;
            
            // When the timer clocks the shift register, the following actions occur in order:
            // Feedback is calculated as the exclusive-OR of bit 0 and one other bit: bit 6 if periodic noise mode (M bit) is set, otherwise bit 1.
            // The shift register is shifted right by one bit.
            // Bit 14, the leftmost bit, is set to the feedback calculated earlier.

            var bit = _periodicNoiseMode ? (_shiftRegister >> 6) & 1 : (_shiftRegister >> 1) & 1;
            var feedback = (_shiftRegister & 1) ^ bit;
            _shiftRegister = (ushort)((_shiftRegister >> 1) | (feedback << 14));
        }
        else
        {
            _timerValue--;
        }
    }

    public void TickLengthCounter()
    {
        if (LengthCounter == 0 || _halt)
        {
            return;
        }

        LengthCounter--;
    }
    
    public void TickEnvelope()
    {
        if (_envelopeStart)
        {
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
            else if (_halt)
            {
                _envelopeCounter = 15;
            }
        }
    }

    public byte GetOutput()
    {
        if (!Enabled || LengthCounter == 0 || (_shiftRegister & 1) == 1)
            return 0;

        return _constantMode ? _volume : _envelopeCounter;
    }
}