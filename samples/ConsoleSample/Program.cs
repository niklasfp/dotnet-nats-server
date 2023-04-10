using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NATS.Client;
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

server.Start();
//
// ConnectionFactory cf = new ConnectionFactory();
// IConnection c = cf.CreateConnection("localhost:4222");
//
// var s = c.ServerInfo;
// Console.WriteLine(s.Version);


Console.WriteLine("Press <ENTER> to exit...");

Console.ReadLine();
server.Stop();

