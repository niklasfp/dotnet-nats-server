using System;
using System.Collections.Generic;

namespace NatsServer.Net
{
    public class NatsServerOptions
    {
        /// <summary>
        /// The arguments to pass to the NATS server.
        /// <para>
        /// The arguments is equal to the commandline arguments supported by the nats server.
        /// <seealso cref="https://docs.nats.io/running-a-nats-service/introduction/flags"/>
        /// </para>
        /// </summary>
        public string[] Arguments { get; set; } = Array.Empty<string>();
    }
}