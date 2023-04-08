using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace NatsServer.Net
{
    /// <summary>
    /// Represents a NATS server instance.
    /// </summary>
    public class NatsServer
    {
        private readonly ILogger<NatsServer> _logger;
        
        private nint _serverPtr;

        /// <summary>
        /// Creates a new instance of <see cref="NatsServer"/>.
        /// </summary>
        /// <param name="logger">The logger instance to use.</param>
        public NatsServer(ILogger<NatsServer> logger)
        {
            _logger = logger;

            // Setup the logger callback.
            NatsServerLib.RegisterLogger(LogMessage);
        }

        /// <summary>
        /// Starts the NATS server.
        /// </summary>
        /// <param name="configFile"></param>
        public void Start(string configFile)
        {
            _serverPtr = NatsServerLib.StartServer(configFile, LogMessage);
        }

        /// <summary>
        /// Shuts down the NATS server.
        /// </summary>
        public void Shutdown()
        {
            if (_serverPtr != 0)
            {
                NatsServerLib.ShutdownServer();
                _serverPtr = 0;
            }
        }

        void LogMessage(string message)
        {
            // Call back from native code
            _logger.LogInformation(message);
        }

    }
}
