using Microsoft.AspNetCore.SignalR;
using PrecisionFullCoilHMI.Services;

public class RealTimeHub : Hub
{
    private readonly OPCUAService _opcuaService;
    private readonly ILogger<RealTimeHub> _logger;

    public RealTimeHub(OPCUAService opcuaService, ILogger<RealTimeHub> logger)
    {
        _opcuaService = opcuaService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
        await _opcuaService.SubscribeToNodes();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToNodes()
    {
        try
        {
            _logger.LogInformation("Client {ConnectionId} is subscribing to nodes", Context.ConnectionId);
            await _opcuaService.SubscribeToNodes();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to nodes for client: {ConnectionId}", Context.ConnectionId);
            await Clients.Caller.SendAsync("ReceiveError", $"Failed to subscribe to nodes: {ex.Message}");
        }
    }
}
