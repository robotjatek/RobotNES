using System.Runtime.InteropServices;
using NESCore.APU;

namespace NESCoreTests.Unit.APUTest;

public class ApuTests : TestBase
{
    private const byte CONTROL_MODE_FLAG = 0x80;
    private const byte CONTROL_INHIBIT_FLAG = 0x40;
    
    [Fact]
    public void GeneratesIrqEveryFrameInFreerunMode()
    {
        // Internally the PPU ticks every second CPU clock
        var sut = new Apu(_logger);
        sut.Write(0x4017, 0); // Freerun mode, IRQ NOT inhibited
        
        ((sut.Read(0x4015) & Apu.STATUS_FRAME_COUNTER_FLAG) > 0).Should().BeFalse(); 

        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        ReadFrameCounterFlag(sut).Should().BeTrue();
    }

    [Fact]
    public void ShouldGenerateIrqContinuously()
    {
        // Internally the PPU ticks every second CPU clock
        var sut = new Apu(_logger);
        sut.Write(0x4017, 0); // Freerun mode, IRQ NOT inhibited
        
        ((sut.Read(0x4015) & Apu.STATUS_FRAME_COUNTER_FLAG) > 0).Should().BeFalse();

        for (var j = 0; j < 10; j++)
        {
            for (var i = 0; i < 29830; i++)
            {
                sut.Tick();
            }

            ReadFrameCounterFlag(sut).Should().BeTrue();
        }
    }
    
    [Fact]
    public void FrameCounterFlagShouldBeDeletedOnStatusRead()
    {
        // same setup as GeneratesIrqEveryFrame, but with an extra read to the status register
        var sut = new Apu(_logger);
        sut.Write(0x4017, 0); // Freerun mode, IRQ NOT inhibited
        
        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        ReadFrameCounterFlag(sut).Should().BeTrue();
        // second read should return false as the first read deleted the flag
        ReadFrameCounterFlag(sut).Should().BeFalse();
    }
    
    [Fact]
    public void IrqShouldNotHappenIfIrqInhibitIsTrue()
    {
        var sut = new Apu(_logger);
        sut.Write(0x4017, CONTROL_INHIBIT_FLAG); // Freerun mode, IRQ inhibited

        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        ReadFrameCounterFlag(sut).Should().BeFalse();
    }
    
    [Fact]
    public void IrqShouldNotHappenInExternalSyncMode()
    {
        var sut = new Apu(_logger);
        sut.Write(0x4017, CONTROL_MODE_FLAG); // External mode (5-step)

        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        ReadFrameCounterFlag(sut).Should().BeFalse();
    }
    
    [Fact]
    public void IrqInhibitShouldClearPendingIrq()
    {
        // Internally the PPU ticks every second CPU clock
        var sut = new Apu(_logger);
        sut.Write(0x4017, 0); // Freerun mode, IRQ NOT inhibited
        
        ((sut.Read(0x4015) & Apu.STATUS_FRAME_COUNTER_FLAG) > 0).Should().BeFalse(); 

        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        sut.Write(0x4017, CONTROL_INHIBIT_FLAG);
        ReadFrameCounterFlag(sut).Should().BeFalse();
    }

    [Fact]
    public void ApuShouldStartIn4StepMode()
    {
        var sut = new Apu(_logger);
        
        ((sut.Read(0x4015) & Apu.STATUS_FRAME_COUNTER_FLAG) > 0).Should().BeFalse(); 

        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        ReadFrameCounterFlag(sut).Should().BeTrue();
    }

    private static bool ReadFrameCounterFlag(Apu apu) => (apu.Read(0x4015) & Apu.STATUS_FRAME_COUNTER_FLAG) > 0;
}