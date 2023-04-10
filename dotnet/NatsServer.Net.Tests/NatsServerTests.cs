using System.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using Options = Microsoft.Extensions.Options.Options;

namespace NatsServer.Net.Tests
{
    public class NatsServerTests
    {
        [Fact]
        public void ShouldBeAbleToStartAndStopServer()
        {
            var sp = BuildServiceProvider();

            var sut = sp.GetRequiredService<NatsServer>();

            sut.Start();

            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection("localhost:4222");

            c.State.Should().Be(ConnState.CONNECTED);
            c.ConnectedId.Should().NotBeNull();
            sut.Stop();
        }

        [Fact]
        public void ShouldBeAbleToStartAndStopServerWithStoppingToken()
        {
            var sp = BuildServiceProvider();

            var sut = sp.GetRequiredService<NatsServer>();

            var cts = new CancellationTokenSource();

            sut.Start(cts.Token);

            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection("localhost:4222");

            c.State.Should().Be(ConnState.CONNECTED);
            c.ConnectedId.Should().NotBeNull();

            cts.Cancel();

            c.State.Should().Be(ConnState.DISCONNECTED);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection()
                .AddLogging(c =>
                    c.AddConsole())
                .AddSingleton<NatsServer>()
                .AddSingleton<IOptions<NatsServerOptions>>(Options.Create(new NatsServerOptions()));

            return services.BuildServiceProvider();
        }
    }
}