using Microsoft.AspNetCore.SignalR;
using OncallAgent.Models;

namespace OncallAgent.Services;

public class IncidentHub : Hub
{
    public async Task SendIncident(ProcessedIncident incident)
    {
        await Clients.All.SendAsync("ReceiveIncident", incident);
    }
}
