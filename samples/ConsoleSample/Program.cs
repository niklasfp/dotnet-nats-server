using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NatsServer.Net;

var services = new ServiceCollection()
    .AddLogging(c =>
        c.AddConsole())
    .AddSingleton<NatsServer.Net.NatsServer>()
    .AddSingleton(new NatsServerOptions()
        {
            Arguments = new[]
            {
                "-js",
            }
        })
        .BuildServiceProvider();

var server = services.GetRequiredService<NatsServer.Net.NatsServer>();

var cts = new CancellationTokenSource();
server.Start(cts.Token);

Console.WriteLine("Press <ENTER> to stop server...");
Console.ReadLine();
cts.Cancel();

Console.WriteLine("Press <ENTER> to exit...");
Console.ReadLine();

