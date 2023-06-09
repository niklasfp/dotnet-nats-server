﻿using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NatsServer.Net
{
    /// <summary>
    /// Represents a NATS server instance.
    /// </summary>
    public sealed class NatsServer: IDisposable
    {
        private readonly ILogger<NatsServer> _logger;
        private readonly NatsServerOptions _options;
        private CancellationTokenSource? _cancelSource;

        private nuint _serverPtr;

        /// <summary>
        /// Creates a new instance of <see cref="NatsServer"/>.
        /// </summary>
        /// <param name="logger">The logger instance to use.</param>
        /// <param name="options">THe server options.</param>
        public NatsServer(ILogger<NatsServer> logger, IOptions<NatsServerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            
            _serverPtr = CreateServerInstance();
            
            if (_serverPtr == 0)
            { 
                throw new NatsServerException("Failed to create NATS server instance, check log for details.");
            }
        }

        /// <summary>
        /// Returns true if the server is running.
        /// </summary>
        public bool IsRunning => _cancelSource != null;
        
        /// <summary>
        /// Starts the NATS server.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the server is already running.</exception>
        /// <exception cref="NatsServerOptions">If the server could not be started.</exception>
        public void Start(CancellationToken stoppingToken = default)
        {
            if (_cancelSource != null)
            {
                throw new InvalidOperationException("Server is already running.");
            }
            
            _logger.LogInformation("Starting server");

            // TODO Error checking
            NatsServerLib.StartServer(_serverPtr);

            if (_serverPtr == 0)
            {
                throw new NatsServerException("Failed to start NATS server, check log for details.");
            }

            _cancelSource = new CancellationTokenSource();
            _cancelSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            _cancelSource.Token.Register(Stop);

            _logger.LogInformation("Server started");
        }

        /// <summary>
        /// Stops the NATS server.
        /// </summary>
        public void Stop()
        {
            if (_cancelSource == null)
            {
                throw new InvalidOperationException("Server is not running.");
            }
            
            _logger.LogInformation("Stopping server");
            
            // TODO Error checking
            NatsServerLib.ShutdownServer(_serverPtr);
                
            _logger.LogInformation("Server stopped");
            
            _cancelSource?.Dispose();
            _cancelSource = null;
        }

        // A callback from native code with log messages.
        // we forward to the logger of this instance with he correct level
        void ForwardLogMessage(string message, long level)
        {
            // Just a precaution, we should never get a level outside of the range
            if (level >= (int)LogLevel.Trace && level <= (int)LogLevel.Critical)
            {
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                _logger.Log((LogLevel)level, message);
                return;
            }
            
            // But we actually did get a level outside the dotnet log levels, so we just log it as information
            _logger.Log(LogLevel.Information, "({Level}) {Message}", level, message);
        }

        private nuint CreateServerInstance()
        {
            // We pass the arguments seperated by a backtick
            var arguments = string.Join('`', _options.Arguments);
            return NatsServerLib.CreateServer(arguments, ForwardLogMessage);
        }

        private void StopAndFree()
        {
            if (_cancelSource != null)
            {
                Stop();
                
                NatsServerLib.FreeServer(_serverPtr);
            }
            
            _serverPtr = 0;
        }

        public void Dispose()
        {
            StopAndFree();
            GC.SuppressFinalize(this);
        }

        ~NatsServer()
        {
            StopAndFree();
        }
    }
}
