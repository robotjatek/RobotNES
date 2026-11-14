using System.Runtime.InteropServices;
using NESCore.APU;

namespace NESCoreTests.Unit.APUTest;

public class ApuTests : TestBase
{
    private const byte ControlModeFlag = 0x80;
    private const byte ControlInhibitFlag = 0x40;
    
    [Fact]
    public void GeneratesIrqEveryFrameInFreerunMode()
    {
        // Internally the PPU ticks every second CPU clock
        var sut = new Apu(_logger);
        sut.Write(0x4017, 0); // Freerun mode, IRQ NOT inhibited

        ReadFrameCounterFlag(sut).Should().BeFalse();

        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        // Fix: needs 3 cycles of extra delay
        ReadFrameCounterFlag(sut).Should().BeFalse();
        for (var i = 0; i < 3; i++)
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
        
        ReadFrameCounterFlag(sut).Should().BeFalse();
        
        // Fix 3 cycle delay
        for (var i = 0; i < 3; i++)
        {
            sut.Tick();
        }

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
        
        // fix: 4 cycle delay when on even phase
        for (var i = 0; i < 4; i++)
        {
            sut.Tick();
        }
        
        for (var i = 0; i < 29830; i++)
        {
            sut.Tick();
        }

        ReadFrameCounterFlag(sut).Should().BeTrue();
        
        sut.Tick();
        sut.Tick();
        // second read should return false as the first read deleted the flag
        ReadFrameCounterFlag(sut).Should().BeFalse();
    }
    
    [Fact]
    public void IrqShouldNotHappenIfIrqInhibitIsTrue()
    {
        var sut = new Apu(_logger);
        sut.Write(0x4017, ControlInhibitFlag); // Freerun mode, IRQ inhibited

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
        sut.Write(0x4017, ControlModeFlag); // External mode (5-step)

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

        sut.Write(0x4017, ControlInhibitFlag);
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