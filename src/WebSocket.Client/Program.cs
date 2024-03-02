using Microsoft.AspNetCore.SignalR.Client;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(sc =>
    {
        sc.AddSingleton<MasterDetailDocumentClient>(sp =>
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5300/ws/chat")
                .AddMessagePackProtocol()
                .Build();

            return new MasterDetailDocumentClient(connection);
        });
    }).Build();

var client = host.Services.GetRequiredService<MasterDetailDocumentClient>();

await client.StartAsync();

var items = await client.GetDocumentItems("test");

var details = await client.GetDocumentItemDetails("test", items.Items[0]);

await client.SetDocumentItemValue("test", items.Items[0], "new value");

await client.GetDocumentItemValue("test", items.Items[0]);