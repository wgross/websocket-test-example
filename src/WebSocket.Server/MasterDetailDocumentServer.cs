using Microsoft.AspNetCore.SignalR.Client;
using WebSocket.Contract;
using WebSocket.Server;

public class MasterDetailDocumentServer
{
    private readonly HubConnection connection;
    private MasterDetailDocument document;

    public MasterDetailDocumentServer(HubConnection connection)
    {
        this.connection = connection;
        this.connection.On<DocumentId, GetDocumentItemsResponse>(nameof(IMasterDetailDocumentProtocol.GetDocumentItems), this.GetDocumentItems);
        this.connection.On<DocumentId, DocumentItem, GetDocumentItemDetailsResponse>(nameof(IMasterDetailDocumentProtocol.GetDocumentItemDetails), this.GetDocumentItemDetails);
        this.connection.On<DocumentId, DocumentItem, string>(nameof(IMasterDetailDocumentProtocol.SetDocumentItemValue), this.SetDocumentItemValue);
        this.connection.On<DocumentId, DocumentItem, string>(nameof(IMasterDetailDocumentProtocol.GetDocumentItemValue), this.GetDocumentItemValue);
    }

    public async Task StartAsync() => await this.connection.StartAsync();

    private Task<string> GetDocumentItemValue(DocumentId id, DocumentItem item)
    {
        return Task.FromResult(this.document[item].Value);
    }

    private Task SetDocumentItemValue(DocumentId id, DocumentItem item, string value)
    {
        this.document[item].Value = value;

        return Task.CompletedTask;
    }

    public Task<GetDocumentItemsResponse> GetDocumentItems(DocumentId documentId)
        => Task.FromResult(new GetDocumentItemsResponse
        {
            Items = this.document.Items.ToArray()
        });

    public Task<GetDocumentItemDetailsResponse> GetDocumentItemDetails(DocumentId documentId, DocumentItem documentItem)
        => Task.FromResult(new GetDocumentItemDetailsResponse
        {
            Details = this.document[documentItem]
        });

    public async Task SetDocumentId(DocumentId documentId, WebSocket.Server.MasterDetailDocument document)
    {
        this.document = document;
        await this.connection.InvokeAsync(nameof(IMasterDetailDocumentProtocol.SetDocumentId), documentId);
    }

    public async Task StopAsync()
    {
        await this.connection.StopAsync();
        await this.connection.DisposeAsync();
    }
}