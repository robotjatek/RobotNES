namespace NESCore.APU;

public class DmcChannel
{
    // The output can be directly set by 0x4011
    private byte _output; // 0-127
    
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
                break;
            case 1:
                // $4011 	-DDD DDDD 	Load counter (D)
                _output = (byte)(value & 0x7F);
                // Direct load register 
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

    public void Tick()
    {
        
    }

    public byte GetOutput()
    {
        // 0-127
        
                    
        return _output;
    }
}