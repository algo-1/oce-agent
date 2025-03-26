using Microsoft.SemanticKernel.Memory;
using OncallAgent.Models;
using OncallAgent.Utils;

namespace OncallAgent.Services;

public class Agent
{
    private readonly ConcurrentPriorityQueue<Incident> _incidentQueue;
    private readonly List<Worker> _workers;
    private readonly CancellationTokenSource _cts;
    private readonly int _maxWorkers;
    private ISemanticTextMemory _memory;
    private string _tsgDir = "./Data/Tsgs";
    private TsgService _tsgService;

    public Agent(int maxWorkers, ISemanticTextMemory memory, TsgService tsgService)
    {
        _maxWorkers = maxWorkers;
        _incidentQueue = new ConcurrentPriorityQueue<Incident>();
        _workers = new List<Worker>();
        _cts = new CancellationTokenSource();
        _memory = memory;
        _tsgService = tsgService;

        // Index TSGs
        Task.Run(() =>
        {
            IndexTsgsAsync(_tsgDir);
        }).ConfigureAwait(false);
    }

    public void Start()
    {
        for (int i = 0; i < _maxWorkers; i++)
        {
            var worker = new Worker(_incidentQueue, i, _tsgService);
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

    public static int GetPriority(Incident incident)
    {
        return incident.Severity switch
        {
            "Sev 1" => 1,
            "Sev 2" => 2,
            "Sev 2.5" => 3,
            "Sev 3" => 4,
            "Sev 4" => 5,
            _ => 6 // Default priority for unknown severity
        };
    }

    public void AddIncident(Incident incident)
    {
        _incidentQueue.Enqueue(GetPriority(incident), incident);
        Console.WriteLine($"Incident added: {incident.Title} with priority {GetPriority(incident)}");
    }

    public async void IndexTsgsAsync(string tsgDirectory)
    {
        await _tsgService.IndexTsgsAsync(tsgDirectory);
    }
}
