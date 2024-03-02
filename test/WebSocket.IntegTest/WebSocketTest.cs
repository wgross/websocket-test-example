using Microsoft.AspNetCore.SignalR.Client;
using WebSocket.Contract;
using WebSocket.Relay;
using WebSocket.Server;

namespace WebSocket.IntegTest;

public class WebSocketTest : IClassFixture<WebSocketTestFixture>, IAsyncDisposable
{
    private readonly WebSocketTestFixture fixture;
    private readonly MasterDetailDocumentServer server;
    private readonly MasterDetailDocumentClient client;
    private readonly string documentId = "test";

    private readonly MasterDetailDocument testDocument;

    public WebSocketTest(WebSocketTestFixture fixture)
    {
        this.fixture = fixture;

        this.testDocument = new MasterDetailDocument([
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
            }]);

        this.server = new MasterDetailDocumentServer(new HubConnectionBuilder()
            .WithUrl(
                url: $"http://localhost:5300/ws/chat",
                configureHttpConnection: c => c.HttpMessageHandlerFactory = _ => this.fixture.CreateHandler())
            .AddMessagePackProtocol()
            .Build());

        this.client = new MasterDetailDocumentClient(new HubConnectionBuilder()
            .WithUrl(
                url: $"http://localhost:5300/ws/chat",
                configureHttpConnection: c => c.HttpMessageHandlerFactory = _ => this.fixture.CreateHandler())
            .AddMessagePackProtocol()
            .Build());
    }

    public async ValueTask DisposeAsync()
    {
        await this.server.StopAsync();
    }

    [Fact]
    public async Task Server_connects_to_relay()
    {
        // ACT
        await this.server.StartAsync();

        // ASSERT
        var result = this.fixture.Services.GetRequiredService<MasterDetailDocumentRelay>();

        Assert.Single(result.KnownConnections);
    }

    [Fact]
    public async Task Server_disconnects_from_relay()
    {
        // ARRANGE
        await this.server.StartAsync();

        // ACT
        await this.server.StopAsync();

        // ASSERT
        var result = this.fixture.Services.GetRequiredService<MasterDetailDocumentRelay>();

        Assert.Empty(result.KnownConnections);
    }

    [Fact]
    public async Task Client_connects_to_relay()
    {
        // ACT
        await this.client.StartAsync();

        // ASSERT
        var result = this.fixture.Services.GetRequiredService<MasterDetailDocumentRelay>();

        Assert.Single(result.KnownConnections);
    }
    
    [Fact]
    public async Task Client_disconnects_from_relay()
    {
        // ARRANGE
        await this.client.StartAsync();

        // ACT
        await this.client.StopAsync();

        // ASSERT
        var result = this.fixture.Services.GetRequiredService<MasterDetailDocumentRelay>();

        Assert.Empty(result.KnownConnections);
    }

    [Fact]
    public async Task Client_reads_document_items()
    {
        // ARRANGE
        await this.server.StartAsync();
        await this.server.SetDocumentId(new DocumentId { Id = documentId }, this.testDocument);

        await this.client.StartAsync();

        // ACT
        var result = await this.client.GetDocumentItems("test");

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(["name1", "name2"], result.Items.Select(i => i.Name));
        Assert.Equal([1, 2], result.Items.Select(i => i.Number));

        var relay = this.fixture.Services.GetRequiredService<MasterDetailDocumentRelay>();

        Assert.Equal(2, relay.KnownConnections.Count());
    }

    [Fact]
    public async Task Client_reads_document_item_details()
    {
        // ARRANGE
        await this.server.StartAsync();
        await this.server.SetDocumentId(new DocumentId { Id = documentId }, this.testDocument);

        await this.client.StartAsync();

        // ACT
        var result = await this.client.GetDocumentItemDetails(documentId, new DocumentItem { Number = 1, Name = "name1" });

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("name1", result.Details.Name);
        Assert.Equal(1, result.Details.Number);
        Assert.Equal("1", result.Details.Value);
    }

    [Fact]
    public async Task Client_set_document_item_details_value()
    {
        // ARRANGE
        await this.server.StartAsync();
        await this.server.SetDocumentId(new DocumentId { Id = documentId }, this.testDocument);

        await this.client.StartAsync();

        // ACT
        await this.client.SetDocumentItemValue(documentId, new DocumentItem { Number = 1, Name = "name1" }, "new");

        var result = await this.client.GetDocumentItemValue(documentId, new DocumentItem { Number = 1, Name = "name1" });

        // ASSERT
        Assert.Equal("new", result);
    }
}