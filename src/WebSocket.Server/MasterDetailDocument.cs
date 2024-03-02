using WebSocket.Contract;

namespace WebSocket.Server;

public class MasterDetailDocument(IEnumerable<DocumentItemDetails> content)
{
    private readonly IDictionary<int, DocumentItemDetails> content = content.ToDictionary(c => c.Number);

    public IEnumerable<DocumentItem> Items => this.content.Values.Select(c => new DocumentItem { Number = c.Number, Name = c.Name });

    public DocumentItemDetails this[DocumentItem item] => this.content[item.Number];
}