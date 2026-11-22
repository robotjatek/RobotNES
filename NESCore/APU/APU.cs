using Serilog;

namespace NESCore.APU
{
    /// <summary>
    /// <remarks>For now only the frame counter IRQ is implemented. IRQ only happens in freerun mode. No auto-ACK from the interrupt handler. F bit stays on until it is cleared manually!</remarks>
    /// </summary>
    public class Apu : IApu
    {
        private readonly ILogger _logger;
        
        /// <summary>
        /// F (frame counter IRQ) bit
        /// </summary>
        public const byte STATUS_FRAME_COUNTER_FLAG = 0x40;
        
        private bool _latch;
        private uint _counter;
        private int _counterResetDelay = -1;
        private bool _clearIrqBit;
        private bool _4StepMode = true;
        private bool _irqInhibit;

        private byte _status;
        
        private const double CpuCyclesPerSample = 1.789773 * 1000 * 1000 / 44100.0; // NTSC cpu clock
        private double _sampleTimer;
        private readonly float[] _sampleBuffer = new float[735]; // 44 100 Hz / 60 FPS = 735
        private int _sampleIndex;
        private float _phase;
        public event IApu.SamplesReadyHandler? SamplesReady;
        
        private readonly PulseChannel _pulseChannel1 = new(true);
        private readonly PulseChannel _pulseChannel2 = new(false);
        private readonly TriangleChannel _triangleChannel = new();
        private readonly NoiseChannel _noiseChannel = new();
        
        public Func<ushort, byte>? MemoryRead { get; set; }
        private readonly DmcChannel _dmcChannel;

        public Apu(ILogger logger)
        {
            _logger = logger;
            _dmcChannel = new DmcChannel(address => MemoryRead?.Invoke(address) ?? 0);
        }
        
        public bool IrqFlag => (_status & STATUS_FRAME_COUNTER_FLAG) != 0 || _dmcChannel.IrqFlag;

        public void Tick()
        {
            _latch = !_latch;

            _triangleChannel.Tick();
            _noiseChannel.Tick();
            _dmcChannel.Tick();
            
            if (!_latch)
            {
                _pulseChannel1.Tick();
                _pulseChannel2.Tick();
            }
            
            if (_clearIrqBit && !_latch)
            {
                _status = unchecked((byte)(_status & (byte)~STATUS_FRAME_COUNTER_FLAG));
                _clearIrqBit = false;
            }
            
            _counter++;
            if (_counterResetDelay > 0)
            {
                _counterResetDelay--;
                if (_counterResetDelay == 0)
                {
                    _counter = 0;
                    _counterResetDelay = -1;
                }
            }
            
            // IRQ only happens in 4-step mode, when IRQs are not inhibited
            if (_4StepMode)
            {
                if (_counter is 7457 or 22371)
                {
                    ClockQuarterFrame();
                }
                else if (_counter is 14913 or 29829)
                {
                    ClockQuarterFrame();
                    ClockHalfFrame();
                }
                
                if (_counter is 29828 or 29829 or 29830 && !_irqInhibit)
                {
                    _status |= STATUS_FRAME_COUNTER_FLAG; // No auto-ACK from the interrupt handler. F bit stays on until it is cleared manually!
                }

                if (_counter >= 29830)
                {
                    _counter = 0;
                }
            }
            else // 5-step mode
            {
                if (_counter is 7457 or 22371)
                {
                    ClockQuarterFrame();
                }
                else if (_counter is 14913 or 37281)
                {
                    ClockQuarterFrame();
                    ClockHalfFrame();
                }
                
                if (_counter >= 37282)
                {
                    _counter = 0;
                }
            }
            
            _sampleTimer += 1.0;
            if (_sampleTimer >= CpuCyclesPerSample)
            {
                _sampleTimer -= CpuCyclesPerSample;
                var triangle = _triangleChannel.GetOutput();
                var noise = _noiseChannel.GetOutput();
                var pulseSum = _pulseChannel1.GetOutput() + _pulseChannel2.GetOutput();
                var dmc = _dmcChannel.GetOutput();
                var tndDenominator = (triangle / 8227.0f) + (noise / 12241.0f) + (dmc / 22638.0f);
                var tOut = tndDenominator == 0f ? 0f : 159.79f / ((1.0f / tndDenominator) + 100.0f);
                var pulseOut = pulseSum == 0 ? 0f : 95.88f / ((8128.0f / pulseSum) + 100.0f); // https://www.nesdev.org/wiki/APU_Mixer
                var sample = (pulseOut + tOut) * 1.5f;
                var filtered = sample - _prevInput + 0.9985f * _prevOutput;
                _prevInput = sample;
                _prevOutput = filtered;
                _sampleBuffer[_sampleIndex++] = Math.Clamp(filtered, -1f, 1f);

                if (_sampleIndex >= _sampleBuffer.Length)
                {
                    SamplesReady?.Invoke(_sampleBuffer, _sampleIndex);
                    _sampleIndex = 0;
                }
            }
        }

        private float _prevInput;
        private float _prevOutput;

        public byte Read(ushort address)
        {
            if (address == 0x4015)
            {
                var result = _status;
                if (_pulseChannel1.LengthCounter > 0) result |= 0x01;
                if (_pulseChannel2.LengthCounter > 0) result |= 0x02;
                if (_triangleChannel.LenghtCounter > 0) result |= 0x04;
                if (_noiseChannel.LengthCounter > 0) result |= 0x08;
                if (_dmcChannel.BytesRemaining > 0) result |= 0x10;
                if (_dmcChannel.IrqFlag) result |= 0x80;
                _clearIrqBit = true; // clear the frame counter bit on the next odd cycle
                return result;
            }
            
            _logger.Debug("Read from APU address 0x{Address:X4} is ignored", address);
            return 0;
        }

        public void Write(ushort address, byte data)
        {
            if (address is >= 0x4000 and <= 0x4003)
            {
                _pulseChannel1.Write((ushort)(address - 0x4000), data);
                return;
            }
            else if (address is >= 0x4004 and <= 0x4007)
            {
                _pulseChannel2.Write((ushort)(address - 0x4004), data);
                return;
            }
            else if (address is >= 0x4008 and <= 0x400B)
            {
                _triangleChannel.Write((ushort)(address - 0x4008), data);
                return;
            }
            else if (address is >= 0x400C and <= 0x400F)
            {
                _noiseChannel.Write((ushort)(address - 0x400C), data);
                return;
            }
            else if (address is >= 0x4010 and <= 0x4013)
            {
                _dmcChannel.Write((ushort)(address - 0x4010), data);
                return;
            }
            else if (address == 0x4015)
            {
                // ---D NT21
                _pulseChannel1.Enabled = (data & 0b1) > 0;
                _pulseChannel2.Enabled = (data & 0b10) > 0;
                _triangleChannel.Enabled = (data & 0b100) > 0;
                _noiseChannel.Enabled = (data & 0b1000) > 0;
                _dmcChannel.Enabled =  (data & 0b10000) > 0;
                // TODO: If the DMC bit is clear, the bytes-remaining counter is set to 0 and the DMC will silence when it empties.
                //  If the DMC bit is set, the DMC sample will be restarted only if its bytes-remaining counter is 0. If there are bits remaining in the shift register, these will finish playing before the next sample is fetched.
                return;
            }
            else if (address == 0x4017)
            {
                // MI-- ----
                // M: 0 = freerun 1 = external
                // I: IRQ inhibit
                
                // 60/120/240 Hz 
                //frame counter register
                
                // NESDEV:
                // Writing to this register resets the counter to 0.
                // If the write occurs during an APU cycle, the effects occur 3 CPU cycles after the write cycle,
                // and if the write occurs between APU cycles, the effects occurs 4 CPU cycles after the write cycle.
                
                _4StepMode = (data & 0b10000000) == 0;
                _irqInhibit = (data & 0b01000000) > 0;
                if (_irqInhibit)
                {
                    _status &= unchecked((byte)~STATUS_FRAME_COUNTER_FLAG);
                }
                // "When the write occurs with the bit 7 (M bit) set, both Quarter frame and Half frame clocks are also generated."
                // "The external synchronization mode (M bit) exists to provide a way for synchronization with the external timing source; in NES' case, it's the Vblank NMI."
                
                // "When the mode is disabled, the frame counter counts its own ≈60Hz cycle that is not synchronized with the Vsync signal from PPU."
                // "When it's enabled, it expects the CPU to manually synchronize the internal counter with the external timing source that the CPU uses."

                // 3 cycles delay to reset when write occurs on odd cycles, 4 on even cycles
                _counterResetDelay = _latch ? 3 : 4;

                if (!_4StepMode)
                {
                    ClockHalfFrame();
                    ClockQuarterFrame();
                }
                
                return;
            }
            _logger.Debug("Write to APU address 0x{Address:X4} is ignored. Value: 0x{Data:X2}", address, data);
        }

        private void ClockHalfFrame()
        {
            _pulseChannel1.TickLenghtCounter();
            _pulseChannel1.TickSweep();
            _pulseChannel2.TickLenghtCounter();
            _pulseChannel2.TickSweep();
            _triangleChannel.TickLengthCounter();
            _noiseChannel.TickLengthCounter();
        }
        
        private void ClockQuarterFrame()
        {
            _pulseChannel1.TickEnvelope();
            _pulseChannel2.TickEnvelope();
            _triangleChannel.TickLinearCounter();
            _noiseChannel.TickEnvelope();
        } 
    }
}
