using Serilog;

namespace NESCore.APU
{
    public class Apu(ILogger logger) : IApu
    {
        private bool _latch;
        private uint _counter; //  TODO: size?
        private const uint _4stepCount = 14915;
        private bool _4stepMode = false;
        private bool _irqInhibit = false;

        public void Tick()
        {
            // TODO: nesdev: framecounter: "The sequencer is clocked on every other CPU cycle, so 2 CPU cycles = 1 APU cycle"
            if (_latch)
            {
                // increment on every second tick
                _counter++;
            }


            if (_4stepMode && _counter == _4stepCount)
            {
                // TODO: irq
                logger.Warning("");
            }
            _latch = !_latch;
                
        }

        public byte Read(ushort address)
        {
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
                
                _4stepMode = (data & 0b10000000) == 0;
                _irqInhibit = (data & 0b01000000) > 0; 
                // "When the write occurs with the bit 7 (M bit) set, both Quarter frame and Half frame clocks are also generated."
                // "The external synchronization mode (M bit) exists to provide a way for synchronization with the external timing source; in NES' case, it's the Vblank NMI."
                
                // "When the mode is disabled, the frame counter counts its own ≈60Hz cycle that is not synchronized with the Vsync signal from PPU."
                // "When it's enabled, it expects the CPU to manually synchronize the internal counter with the external timing source that the CPU uses."

                _counter = 0; // TODO: delay after write
            }
            logger.Debug("Write to APU address 0x{Address:X4} is ignored. Value: 0x{Data:X2}", address, data);
        }
    }
}
