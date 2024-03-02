namespace WebSocket.IntegTest;

public class WebSocketTestFixture : IAsyncLifetime
{
    private readonly RelayTestHostFactory factory;

    public WebSocketTestFixture()
    {
        this.factory = new RelayTestHostFactory();
    }

    public Uri BaseAddress => this.factory.Server.BaseAddress;

    public IServiceProvider Services => this.factory.Services;

    public async Task InitializeAsync()
    {
        await Task.Yield();
    }

    public async Task DisposeAsync() => await this.factory.DisposeAsync();

    public HttpMessageHandler CreateHandler()
        => this.factory.Server.CreateHandler();
}