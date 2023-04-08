using System.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace NatsServer.Net.Tests
{
    public class NatsServerTests
    {
        [Fact]
        public void ShouldBeAbleToStartAndStopServer()
        {
            var services = new ServiceCollection()
                .AddLogging(c =>
                    c.AddConsole())
                .AddSingleton<NatsServer>()
                .BuildServiceProvider();

            var server = services.GetRequiredService<NatsServer>();

            server.Start("nats.conf");

            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection("localhost:4222");

            c.State.Should().Be(ConnState.CONNECTED);
            c.ConnectedId.Should().NotBeNull();
            server.Shutdown();
        }
    }
}