using Microsoft.AspNetCore.SignalR.Client;
using WebSocket.Contract;
using WebSocket.Server;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(sc =>
    {
        sc.AddSingleton<MasterDetailDocumentServer>(sc =>
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5300/ws/chat")
                .AddMessagePackProtocol()
                .Build();

            return new MasterDetailDocumentServer(connection);
        });
    })
    .Build();

var server = host.Services.GetRequiredService<MasterDetailDocumentServer>();

await server.StartAsync();

await server.SetDocumentId(new DocumentId() { Id = "test" }, new MasterDetailDocument([
        new()
        {
            Name = "name1",
            Number = 1,
            Value = "1"
        },
        new()
        {
            Number = 2,
            Name = "name2",
            Value = "1"
        }
    ]));

await Task.Delay(TimeSpan.FromSeconds(60));