using OncallAgent.Models;
using OncallAgent.Utils;

namespace OncallAgent.Services;

public class Agent
{
    private readonly ConcurrentPriorityQueue<Incident> _incidentQueue;
    private readonly List<Worker> _workers;
    private readonly CancellationTokenSource _cts;
    private readonly int _maxWorkers;

    public Agent(int maxWorkers)
    {
        _maxWorkers = maxWorkers;
        _incidentQueue = new ConcurrentPriorityQueue<Incident>();
        _workers = new List<Worker>();
        _cts = new CancellationTokenSource();
    }

    public void Start()
    {
        for (int i = 0; i < _maxWorkers; i++)
        {
            var worker = new Worker(_incidentQueue, i);
            _workers.Add(worker);
            worker.Start();
        }
    }

    public void Stop()
    {
        foreach (var worker in _workers)
        {
            worker.Stop();
        }

        _cts.Cancel();
        _cts.Dispose();
        Console.WriteLine("All workers stopped.");
    }

    public void StartWorker(int workerId)
    {
        if (workerId < 0 || workerId >= _workers.Count)
        {
            Console.WriteLine("Invalid worker ID.");
            return;
        }

        _workers[workerId].Start();
    }

    public void StopWorker(int workerId)
    {
        if (workerId < 0 || workerId >= _workers.Count)
        {
            Console.WriteLine("Invalid worker ID.");
            return;
        }

        _workers[workerId].Stop();
    }

    public void ClearIncidents()
    {
        _incidentQueue.Clear();
        Console.WriteLine("Incident queue cleared.");
    }

    public int GetPriority(Incident incident)
    {
        return incident.Severity switch
        {
            "Critical" => 1,
            "High" => 2,
            "Medium" => 3,
            "Low" => 4,
            _ => 5 // Default priority for unknown severity
        };
    }

    public void AddIncident(Incident incident)
    {
        _incidentQueue.Enqueue(GetPriority(incident), incident);
        Console.WriteLine($"Incident added: {incident.Title} with priority {GetPriority(incident)}");
    }

}
