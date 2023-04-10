using System;

namespace NatsServer.Net
{
    public class NatsServerException : Exception
    {
        public NatsServerException()
        {
        }

        public NatsServerException(string message) : base(message)
        {
        }

        public NatsServerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}