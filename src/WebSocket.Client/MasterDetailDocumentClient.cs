using Microsoft.AspNetCore.SignalR.Client;
using WebSocket.Contract;

// await Task.Delay(TimeSpan.FromSeconds(60));

public class MasterDetailDocumentClient
{
    private readonly HubConnection connection;

    public MasterDetailDocumentClient(HubConnection connection)
    {
        this.connection = connection;
    }

    public async Task StartAsync() => await this.connection.StartAsync();

    public async Task<GetDocumentItemsResponse> GetDocumentItems(string documentId)
        => await this.connection.InvokeAsync<GetDocumentItemsResponse>(nameof(IMasterDetailDocumentProtocol.GetDocumentItems), new DocumentId { Id = documentId });

    public async Task<GetDocumentItemDetailsResponse> GetDocumentItemDetails(string documentId, DocumentItem documentItem)
        => await this.connection.InvokeAsync<GetDocumentItemDetailsResponse>(nameof(GetDocumentItemDetails), new DocumentId() { Id = documentId }, documentItem);

    public async Task SetDocumentItemValue(string documentId, DocumentItem documentItem, string documentItemValue)
        => await this.connection.InvokeAsync(nameof(IMasterDetailDocumentProtocol.SetDocumentItemValue), new DocumentId { Id = documentId }, documentItem, documentItemValue);

    public async Task<string> GetDocumentItemValue(string documentId, DocumentItem documentItem)
        => await this.connection.InvokeAsync<string>(nameof(IMasterDetailDocumentProtocol.GetDocumentItemValue), new DocumentId { Id = documentId }, documentItem);

    public async Task StopAsync() => await this.connection.StopAsync();
}