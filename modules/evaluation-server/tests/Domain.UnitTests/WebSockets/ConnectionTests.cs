﻿using System.Net.WebSockets;
using Domain.WebSockets;
using Moq;

namespace Domain.UnitTests.WebSockets;

public class ConnectionTests
{
    private readonly Mock<WebSocket> _webSocketMock = new();
    private readonly Connection _connection;

    public ConnectionTests()
    {
        _connection =
            new Connection(_webSocketMock.Object, 1, ConnectionType.Client, ConnectionVersion.V1, 1662395291241);
    }

    [Fact]
    public void CreateConnection()
    {
        _webSocketMock.Setup(x => x.State).Returns(WebSocketState.Open);

        Assert.True(Guid.TryParse(_connection.Id, out _));
        Assert.Equal(WebSocketState.Open, _connection.WebSocket.State);
        Assert.Equal(1, _connection.EnvId);
        Assert.Equal(ConnectionType.Client, _connection.Type);
        Assert.Equal(ConnectionVersion.V1, _connection.Version);
        Assert.Equal(1662395291241, _connection.ConnectAt);
        Assert.Equal(0, _connection.CloseAt);
    }

    [Fact]
    public void ConnectionToString()
    {
        _webSocketMock.Setup(x => x.State).Returns(WebSocketState.Open);
        
        Assert.Equal(
            $"id={_connection.Id},envId=1,sdkType=client,version=1,status=Open,connectAt=1662395291241,closeAt=0",
            _connection.ToString()
        );
    }

    [Fact]
    public async Task CloseConnection()
    {
        await _connection.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, 1662904887947);

        _webSocketMock.Verify(
            ws => ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None),
            Times.Once
        );

        Assert.Equal(1662904887947, _connection.CloseAt);
    }
}