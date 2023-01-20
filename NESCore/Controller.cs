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
            _logger.Warning("Controller read is ignored");
            return 0;
        }

        public void Reset()
        {
            _logger.Warning("Controller reset is ignored");
        }
    }
}
