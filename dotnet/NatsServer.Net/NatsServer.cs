using System;
using System.Buffers.Binary;
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
        private readonly NatsServerOptions _options;

        private nuint _serverPtr;

        /// <summary>
        /// Creates a new instance of <see cref="NatsServer"/>.
        /// </summary>
        /// <param name="logger">The logger instance to use.</param>
        /// <param name="options">THe server options.</param>
        public NatsServer(ILogger<NatsServer> logger, NatsServerOptions options)
        {
            _logger = logger;
            _options = options;

            // Setup the logger callback.
            // TODO: Make this go along with the server creation
            NatsServerLib.RegisterLogger(LogMessage);

            _serverPtr = CreateServer();
        }

        /// <summary>
        /// Starts the NATS server.
        /// </summary>
        /// <param name="configFile"></param>
        public void Start(string configFile)
        {
            _logger.LogInformation("Starting server");
            if (_serverPtr != 0)
            {
                // TODO Error checking
                NatsServerLib.StartServer(_serverPtr);
            }
            // TODO: Raise here
        }

        
        /// <summary>
        /// Shuts down the NATS server.
        /// </summary>
        public void Shutdown()
        {
            if (_serverPtr != 0)
            {
                NatsServerLib.ShutdownServer(_serverPtr);
                _serverPtr = 0;
            }
            
            // TODO: Raise here
        }

        void LogMessage(string message, long level)
        {
            // Call back from native code
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            if (level >= (int)LogLevel.Trace && level <= (int)LogLevel.Critical)
            {
                _logger.Log((LogLevel)level, message);
                return;
            }
            
            _logger.Log(LogLevel.Information, "({level}) {message}", level, message);
        }

        private nuint CreateServer()
        {
            // We pass the arguments seperated by a backtick
            var arguments = string.Join('`', _options.Arguments);
            return NatsServerLib.CreateServer(arguments);
        }
    }
}
