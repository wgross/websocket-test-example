using Microsoft.AspNetCore.SignalR;
using WebSocket.Contract;

namespace WebSocket.Relay;
public class MasterDetailProtocolHub(MasterDetailDocumentRelay manager) : Hub<IMasterDetailDocumentProtocol>, IMasterDetailDocumentProtocol
{
    private readonly MasterDetailDocumentRelay manager = manager;

    public override Task OnConnectedAsync()
    {
        this.manager.AddProtocolInstance(Context.ConnectionId);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        this.manager.RemoveProtocolInstance(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    public Task SetDocumentId(DocumentId documentId)
    {
        this.manager.SetDocumentId(Context.ConnectionId, documentId.Id);
        return Task.CompletedTask;
    }

    public async Task<GetDocumentItemsResponse> GetDocumentItems(DocumentId documentId)
    => await this.manager.GetDocumentItems(documentId);

    public async Task<GetDocumentItemDetailsResponse> GetDocumentItemDetails(DocumentId documentId, DocumentItem documentItem)
        => await this.manager.GetDocumentItemDetails(documentId, documentItem);

    public async Task SetDocumentItemValue(DocumentId documentId, DocumentItem documentItem, string documentItemValue)
        => await this.manager.SetDocumentItemValue(documentId, documentItem, documentItemValue);

    public async Task<string> GetDocumentItemValue(DocumentId documentId, DocumentItem documentItem)
        => await this.manager.GetDocumentItemValue(documentId, documentItem);
}