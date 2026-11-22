namespace NESCore.APU;

public class DmcChannel(Func<ushort, byte> busRead)
{
    // how many cpu cycles to wait before switching to the next bit in the sample
    private static readonly ushort[] RateTable = [428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106,  84,  72,  54];
    
    // The output can be directly set by 0x4011
    private byte _output; // 0-127
    
    private ushort _timerValue;
    private ushort _timerPeriod = RateTable[0];
    private bool _loop;
    private bool _irqEnabled;
    
    public bool Enabled
    { 
        get;
        set
        {
            if (!value)
            {
                BytesRemaining = 0;
            }
            else if (BytesRemaining == 0)
            {
                RestartSample();
                FillSampleBuffer();
            }
            field = value;
        }
    }

    private void RestartSample()
    {
        _currentAddress = _startAddress;
        BytesRemaining = _sampleLength;
    }

    private void FillSampleBuffer()
    {
        if (!_sampleBufferEmpty || BytesRemaining == 0)
            return;

        _sampleBuffer = busRead(_currentAddress);
        _sampleBufferEmpty = false;
        
        _currentAddress = _currentAddress == 0xFFFF ? (ushort)0x8000 : (ushort)(_currentAddress + 1);
        BytesRemaining--;

        if (BytesRemaining == 0)
        {
            if (_loop)
            {
                RestartSample();
            }
            else if (_irqEnabled)
            {
                IrqFlag = true;
            }
        }
    }

    public void Write(ushort register, byte value)
    {
        switch (register)
        {
            case 0:
                // $4010 	IL-- RRRR 	DMC sample-end IRQ enable (I), Loop (L), Sample rate (R)
                var rate = RateTable[value & 0x0f];
                _timerPeriod = rate; // how many cpu cycles to wait before switching to the next bit in the sample
                _loop = (value & 0b0100_0000) > 0;
                _irqEnabled = (value & 0b1000_0000) > 0;
                if (!_irqEnabled)
                {
                    IrqFlag = false;
                }
                break;
            case 1:
                // $4011 	-DDD DDDD 	Load counter (D)
                _output = (byte)(value & 0x7F); // Direct load register 
                break;
            case 2:
                // $4012 	AAAA AAAA 	Sample address (A) 
                _startAddress = (ushort)(0xC000 + value * 64);
                break;
            case 3:
                // $4013 	LLLL LLLL 	Sample length (L)
                _sampleLength = (ushort)(value * 16 + 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(register), register, null);
        }
    }
    
    private ushort _startAddress = 0xC000;
    private ushort _currentAddress = 0xC000;
    private ushort _sampleLength = 1;
    public ushort BytesRemaining { get; private set; }
    public bool IrqFlag { get; private set; }
    
    private bool _sampleBufferEmpty = true;
    private byte _sampleBuffer;
    private byte _shiftRegister;
    private int _remainingBits = 8;
    private bool _silence = true;

    // TODO: why hardcoded rates?
    public void Tick()
    {
        if (_timerValue > 0)
        {
            _timerValue--;
            return;
        }
        
        _timerValue = (ushort)(_timerPeriod - 1);
        
        // move shift register based on the timer
        if (!_silence)
        {
            // get the next bit from the shift register when the timer runs out
            // output moves +2/-2 every tick
            var lsb = _shiftRegister & 1;
            if (lsb == 1)
            {
                if (_output <= 125)
                    _output += 2;
            }
            else
            {
                if (_output >= 2)
                    _output -= 2;
            }
        }
        
        _shiftRegister = (byte)(_shiftRegister >> 1);
        _remainingBits--;

        if (_remainingBits == 0)
        {
            _remainingBits = 8;
            if (_sampleBufferEmpty)
            {
                _silence = true;
            }
            else
            {
                _silence = false;
                _shiftRegister = _sampleBuffer;
                _sampleBufferEmpty = true;
                FillSampleBuffer();
            }
        }
    }

    public byte GetOutput()
    {
        // 0-127
        return (byte)(_output);
    }
}