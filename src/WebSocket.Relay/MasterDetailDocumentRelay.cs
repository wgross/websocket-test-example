using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using WebSocket.Contract;

namespace WebSocket.Relay;

public class MasterDetailDocumentRelay(IHubContext<MasterDetailProtocolHub, IMasterDetailDocumentProtocol> context)
{
    private readonly IHubContext<MasterDetailProtocolHub, IMasterDetailDocumentProtocol> context = context;

    private readonly ConcurrentDictionary<string, string> connectionId2DocumentId = new();
    private readonly ConcurrentDictionary<string, string> documentId2connectionId = new();
    private readonly ConcurrentDictionary<string, IMasterDetailDocumentProtocol> connectionId2protocolInstance = new();

    public void AddProtocolInstance(string connectionId)
    {
        this.connectionId2protocolInstance[connectionId] = this.context.Clients.Client(connectionId);
    }

    public IEnumerable<string> KnownConnections => this.connectionId2protocolInstance.Keys;

    public IEnumerable<string> KnownDocumentIds => this.documentId2connectionId.Keys;

    public void RemoveProtocolInstance(string connectionId)
    {
        this.connectionId2protocolInstance.Remove(connectionId, out _);

        if (this.connectionId2DocumentId.Remove(connectionId, out var documentId))
            this.documentId2connectionId.Remove(documentId, out _);
    }

    public void SetDocumentId(string connectionId, string id)
    {
        if (this.connectionId2DocumentId.TryAdd(connectionId, id))
            this.documentId2connectionId.TryAdd(id, connectionId);
    }

    public IMasterDetailDocumentProtocol GetConnectionByDocumentId(DocumentId documentId) => this.GetConnectionByDocumentId(documentId.Id);

    public IMasterDetailDocumentProtocol GetConnectionByDocumentId(string documentId)
    {
        if (this.documentId2connectionId.TryGetValue(documentId, out var connectionId))
            if (this.connectionId2protocolInstance.TryGetValue(connectionId, out var protocolInstance))
                return protocolInstance;
        throw new KeyNotFoundException(documentId);
    }

    public async Task<GetDocumentItemsResponse> GetDocumentItems(DocumentId documentId)
        => await this.GetConnectionByDocumentId(documentId).GetDocumentItems(documentId);

    public async Task<GetDocumentItemDetailsResponse> GetDocumentItemDetails(DocumentId documentId, DocumentItem documentItem)
        => await this.GetConnectionByDocumentId(documentId).GetDocumentItemDetails(documentId, documentItem);

    public async Task SetDocumentItemValue(DocumentId documentId, DocumentItem documentItem, string documentItemValue)
        => await this.GetConnectionByDocumentId(documentId).SetDocumentItemValue(documentId, documentItem, documentItemValue);

    public async Task<string> GetDocumentItemValue(DocumentId documentId, DocumentItem documentItem)
        => await this.GetConnectionByDocumentId(documentId).GetDocumentItemValue(documentId, documentItem);
}