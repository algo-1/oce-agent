using OncallAgent.Models;
using OncallAgent.Utils;

namespace OncallAgent.Services;

public class Worker
{
    private readonly ConcurrentPriorityQueue<Incident> _incidentQueue;
    private readonly int _workerId;
    private readonly CancellationTokenSource _cts;

    public Worker(ConcurrentPriorityQueue<Incident> incidentQueue, int workerId)
    {
        _incidentQueue = incidentQueue;
        _workerId = workerId;
        _cts = new CancellationTokenSource();
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

                // TODO: Replace with actual processing logic
                await Task.Delay(1000, token);
                // TODO: Replace with actual processing logic

                Console.WriteLine($"Worker {_workerId} finished processing incident: {incident.Title}");
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
