using MessagePack;

namespace WebSocket.Contract;

[MessagePackObject]
public class DocumentId
{
    [Key(0)]
    public string Id { get; set; }
}

[MessagePackObject]
public class GetDocumentItemsResponse
{
    [Key(0)]
    public DocumentItem[] Items { get; set; }
}

[MessagePackObject]
public class GetDocumentItemDetailsResponse
{
    [Key(0)]
    public DocumentItemDetails Details { get; set; }
}

[MessagePackObject]
public class DocumentItem
{
    [Key(0)]
    public int Number { get; set; }

    [Key(1)]
    public string Name { get; set; }
}

[MessagePackObject]
public class DocumentItemDetails
{
    [Key(0)]
    public int Number { get; set; }

    [Key(1)]
    public string Name { get; set; }

    [Key(2)]
    public string Value { get; set; }
}