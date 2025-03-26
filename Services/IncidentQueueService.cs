

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using OncallAgent.Models;
using OncallAgent.Utils;

namespace OncallAgent.Services;

public class ProcessedIncidentQueueService : BackgroundService
{
    private static readonly ConcurrentPriorityQueue<ProcessedIncident> _incidentQueue = new();
    private readonly IHubContext<IncidentHub> _hubContext;

    public ProcessedIncidentQueueService(IHubContext<IncidentHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public static void EnqueueIncident(ProcessedIncident incident, int priority)
    {
        _incidentQueue.Enqueue(priority, incident);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_incidentQueue.IsEmpty())
                {
                    var incident = _incidentQueue.Dequeue();
                    Console.WriteLine($"Sending incident : {incident.Title}");

                    // Notify all clients about the new incident
                    await _hubContext.Clients.All.SendAsync("ReceiveIncident", incident);
                }
                else
                {
                    await Task.Delay(100); // Sleep before checking the queue again
                }
            }
        }, stoppingToken);
    }
}
