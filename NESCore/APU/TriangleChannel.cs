namespace NESCore.APU;

public class TriangleChannel
{
    private bool _halt;
    private ushort _timerPeriod;
    private ushort _timerValue;
    private int _sequenceStep;
    private byte _linearCounter;
    private byte _linearReloadValue;
    private bool _linearReload;
    
    public byte LenghtCounter { get; private set; }
    public bool Enabled { 
        get;
        set
        {
            field = value;
            if (!field)
            {
                LenghtCounter = 0;
            }
        }
    }
    
    private static readonly byte[] LengthCounterTable = [10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14, 12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30]; // from nesdev wiki

    // Similar to the Duty table of the pulse channel but it has 32 steps instead of 8 and has no concept of duty cycles
    private static readonly byte[] SequenceTable =
    [
        15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
    ];
    
    public void Write(ushort register, byte value)
    {
        switch (register)
        {
            case 0:
                _linearReloadValue = (byte)(value & 0x7F);
                _halt = (value & 0x80) > 0;
                break;
            case 1:
                //unused
            break;
            case 2: //timer low
                _timerPeriod = (ushort)((_timerPeriod & 0x0700) | value);
                break;
            case 3: //LLLL LTTT
                _timerPeriod = (ushort)(( _timerPeriod & 0x00ff) | (value & 0b111) << 8);
                if (Enabled)
                    LenghtCounter = LengthCounterTable[(value >> 3) & 0b11111];
                _linearReload = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(register), register, null);
        }
    }

    public void Tick()
    {
        if (_timerValue == 0)
        {
            _timerValue = _timerPeriod;
            if (LenghtCounter > 0 && _linearCounter > 0)
            {
                _sequenceStep = (byte)(_sequenceStep + 1) & 0x1F;
            }
        }
        else
        {
            _timerValue--;
        }
    }

    public void TickLengthCounter()
    {
        if (LenghtCounter == 0 || _halt)
            return;

        LenghtCounter--;
    }

    public void TickLinearCounter()
    {
        if (_linearReload)
        {
            _linearCounter = _linearReloadValue;
        }
        else if (_linearCounter > 0)
        {
            _linearCounter--;
        }

        if (!_halt)
        {
            _linearReload = false;
        }
    }
    
    public byte GetOutput()
    {
        if (_timerPeriod < 2)
            return 0;
        return SequenceTable[_sequenceStep];
    }
}