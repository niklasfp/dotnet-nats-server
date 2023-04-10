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
            sut.IsRunning.Should().BeTrue();

            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection("localhost:4222");

            c.State.Should().Be(ConnState.CONNECTED);
            c.ConnectedId.Should().NotBeNull();

            sut.Stop();
            sut.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void ShouldBeAbleToStartAndStopServerWithStoppingToken()
        {
            var sp = BuildServiceProvider();

            var sut = sp.GetRequiredService<NatsServer>();

            var cts = new CancellationTokenSource();

            sut.Start(cts.Token);

            sut.IsRunning.Should().BeTrue();

            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection("localhost:4222");

            c.State.Should().Be(ConnState.CONNECTED);
            c.ConnectedId.Should().NotBeNull();

            cts.Cancel();

            sut.IsRunning.Should().BeFalse();
        }
        
        // Test that trying to stop a server that is not running throws an exception
        [Fact]
        public void ShouldThrowExceptionWhenStoppingServerThatIsNotRunning()
        {
            var sp = BuildServiceProvider();

            var sut = sp.GetRequiredService<NatsServer>();

            sut.Invoking(s => s.Stop()).Should().Throw<InvalidOperationException>();
        }
        
        // Test that trying to start a server that is already running throws an exception
        [Fact]
        public void ShouldThrowExceptionWhenStartingServerThatIsAlreadyRunning()
        {
            var sp = BuildServiceProvider();

            var sut = sp.GetRequiredService<NatsServer>();

            sut.Start();
            sut.Invoking(s => s.Start()).Should().Throw<InvalidOperationException>();
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