using WebSocket.Contract;

namespace WebSocket.Relay;

public class RelayMasterDetailDocumentClient(MasterDetailDocumentRelay relay)
{
    private readonly MasterDetailDocumentRelay relay = relay;

    public async Task<GetDocumentItemsResponse> GetDocumentItems(string documentId)
    {
        return await this.relay.GetDocumentItems(new DocumentId { Id = documentId });
    }

    public async Task<GetDocumentItemDetailsResponse> GetDocumentItemDetails(string documentId, DocumentItem documentItem)
    {
        var docId = new DocumentId { Id = documentId };

        return await this.relay.GetDocumentItemDetails(docId, documentItem);
    }

    public async Task SetDocumentItemValue(string documentId, DocumentItem documentItem, string value)
    {
        await this.relay.SetDocumentItemValue(new DocumentId { Id = documentId }, documentItem, value);
    }

    public async Task<IEnumerable<char>?> GetDocumentItemValue(string documentId, DocumentItem documentItem)
    {
        return await this.relay.GetDocumentItemValue(new DocumentId { Id = documentId }, documentItem);
    }
}