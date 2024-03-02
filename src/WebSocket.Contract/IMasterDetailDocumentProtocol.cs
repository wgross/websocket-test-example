namespace WebSocket.Contract;

public interface IMasterDetailDocumentProtocol
{
    Task SetDocumentId(DocumentId id);

    Task<GetDocumentItemsResponse> GetDocumentItems(DocumentId documentId);

    Task<GetDocumentItemDetailsResponse> GetDocumentItemDetails(DocumentId documentId, DocumentItem documentItem);

    Task SetDocumentItemValue(DocumentId documentId, DocumentItem documentItem, string documentItemValue);

    Task<string> GetDocumentItemValue(DocumentId documentId, DocumentItem documentItem);
}
