using MessagePack;
using WebSocket.Relay;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<MasterDetailDocumentRelay>();

// add the binary message pack protocol instead of JSON
builder.Services
    .AddSignalR()
    .AddMessagePackProtocol(options =>
    {
        options.SerializerOptions = MessagePackSerializerOptions.Standard
            .WithSecurity(MessagePackSecurity.UntrustedData);
    });

builder.Services.AddSingleton<RelayMasterDetailDocumentClient>();

// start the server at port 5200 and make the hub accessible at URL /ws/chat
var app = builder.Build();
app.Urls.Add("http://localhost:5300");
app.MapHub<MasterDetailProtocolHub>("/ws/chat");
app.Run();