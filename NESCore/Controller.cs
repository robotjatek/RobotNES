using Serilog;

namespace NESCore
{
    /**
     * 0 - A
     * 1 - B
     * 2 - Select
     * 3 - Start
     * 4 - Up
     * 5 - Down
     * 6 - Left
     * 7 - Right
     */
    internal class Controller : IController
    {
        private readonly ILogger _logger;
        private byte _buttonStates;
        private byte _shift;
        private bool _strobe;

        public Controller(ILogger logger)
        {
            _logger = logger;
        }

        public void PressKey(IController.Keys key)
        {
            _buttonStates |= (byte)key;
        }
        
        public void ReleaseKey(IController.Keys key)
        {
            _buttonStates &= (byte)~key;
        }

        public byte ReadNextButton()
        {
            CopyStateToShiftRegister();
            
            // return the least significant bit
            var key = (byte)(_shift & 0x01);

            if (!_strobe)
            {
                _shift >>= 1;
                _shift |= 0x80;
            }
            
            return key;
        }

        public void StrobeWrite(byte value)
        {
            _strobe = (value & 0x1) == 1;
            CopyStateToShiftRegister();
        }
        
        private void CopyStateToShiftRegister()
        {
            if (_strobe)
            {
                _shift = _buttonStates;
            }
        }
    }
}
