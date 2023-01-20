using Serilog;

namespace NESCore
{
    internal class Controller : IController
    {
        private readonly ILogger _logger;

        public Controller(ILogger logger) 
        {
            _logger = logger;
        }

        public byte ReadNextButton()
        {
            _logger.Debug("Controller read is ignored");
            return 0;
        }

        public void Reset()
        {
            _logger.Debug("Controller reset is ignored");
        }
    }
}
