namespace NESCore.APU;

public class DmcChannel
{
    // how many cpu cycles to wait before switching to the next bit in the sample
    private static readonly ushort[] RateTable = [428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106,  84,  72,  54];
    
    // The output can be directly set by 0x4011
    private byte _output; // 0-127
    
    private ushort _timerValue;
    private ushort _timerPeriod;
    private bool _loop;
    private bool _irq;
    
    public bool Enabled
    { 
        get;
        set
        {
            field = value;
            // TODO: length counter?
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
                _irq = (value & 0b1000_0000) > 0;
                break;
            case 1:
                // $4011 	-DDD DDDD 	Load counter (D)
                _output = (byte)(value & 0x7F); // Direct load register 
                break;
            case 2:
                // $4012 	AAAA AAAA 	Sample address (A) 
                break;
            case 3:
                // $4013 	LLLL LLLL 	Sample length (L) 
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(register), register, null);
        }
    }
    
    // TODO: load shift register
    private byte _shiftRegister;
    private int _remainingBits;
    private bool _silence = true;

    // TODO: why hardcoded rates?
    public void Tick()
    {
        if (_timerValue > 0)
        {
            _timerValue--;
            return;
        }
        
        _timerValue = _timerPeriod;
        
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
            _shiftRegister = (byte)(_shiftRegister >> 1);
            _remainingBits--;

            if (_remainingBits == 0)
            {
                // TODO: reload shift register
                _remainingBits = 8;
                _silence = true;
            }
        }
    }

    public byte GetOutput()
    {
        // 0-127
        return _output;
    }
}