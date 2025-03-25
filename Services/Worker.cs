using OncallAgent.Models;
using OncallAgent.Utils;

namespace OncallAgent.Services;

public class Worker
{
    private readonly ConcurrentPriorityQueue<Incident> _incidentQueue;
    private readonly int _workerId;
    private readonly CancellationTokenSource _cts;
    private readonly TsgService _tsgService;

    public Worker(ConcurrentPriorityQueue<Incident> incidentQueue, int workerId, TsgService tsgService)
    {
        _incidentQueue = incidentQueue;
        _workerId = workerId;
        _cts = new CancellationTokenSource();
        _tsgService = tsgService;
    }

    public void Start()
    {
        Task.Run(() => ProcessIncidents(_cts.Token));
    }

    private async Task ProcessIncidents(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!_incidentQueue.IsEmpty())
            {
                var incident = _incidentQueue.Dequeue();
                Console.WriteLine($"Worker {_workerId} processing incident: {incident.Title}");

                // TODO: Update processing logic
                // query the TSG service for relevant TSGs
                var relevantTsg = await _tsgService.RetrieveRelevantTsgAsync(incident.Description);
                if (relevantTsg is not null)
                {
                    Console.WriteLine($"Worker {_workerId} found relevant TSG: {relevantTsg} for incident: {incident.Description}");
                }
                else
                {
                    Console.WriteLine($"Worker {_workerId} found no relevant TSG for incident {incident.Description}.");
                }

                Console.WriteLine($"Worker {_workerId} finished processing incident: {incident.Description}");
            }
            else
            {
                await Task.Delay(500, token); // Wait before checking the queue again
            }
        }
    }

    public void Stop()
    {
        _cts.Cancel();
        _cts.Dispose();
        Console.WriteLine($"Worker {_workerId} stopped.");
    }

}
