using Serilog;

namespace NESCore.APU
{
    /// <summary>
    /// <remarks>For now only the frame counter IRQ is implemented. IRQ only happens in freerun mode. No auto-ACK from the interrupt handler. F bit stays on until it is cleared manually!</remarks>
    /// </summary>
    /// <param name="logger"></param>
    public class Apu(ILogger logger) : IApu
    {
        /// <summary>
        /// F (frame counter IRQ) bit
        /// </summary>
        public const byte STATUS_FRAME_COUNTER_FLAG = 0x40;
        
        private bool _latch;
        private uint _counter;
        private bool _4StepMode = true;
        private bool _irqInhibit;

        private byte _status;
        public bool IrqFlag => (_status & STATUS_FRAME_COUNTER_FLAG) != 0;

        public void Tick()
        {
            _counter++;
            
            // IRQ only happens in 4-step mode, when IRQs are not inhibited
            if (_4StepMode)
            {
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
                if (_counter >= 37282)
                {
                    _counter = 0;
                }
            }
            
            _latch = !_latch;
        }

        public byte Read(ushort address)
        {
            if (address == 0x4015)
            {
                var result = _status;
                // clear the frame counter bit
                _status = unchecked((byte)(_status & (byte)~STATUS_FRAME_COUNTER_FLAG));
                return result;
            }
            
            logger.Debug("Read from APU address 0x{Address:X4} is ignored", address);
            return 0;
        }

        public void Write(ushort address, byte data)
        {
            if (address == 0x4017)
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

                _counter = 0; // TODO: delay after write
                
                return;
            }
            logger.Debug("Write to APU address 0x{Address:X4} is ignored. Value: 0x{Data:X2}", address, data);
        }
    }
}
