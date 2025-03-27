using Microsoft.Extensions.Hosting;
using OncallAgent.Models;

namespace OncallAgent.Services;

public class IncidentSimulationService : BackgroundService
{
    private readonly Agent _agent;
    private readonly Random _random = new();
    private readonly List<Incident> _incidentTemplates = new()
    {
        new Incident
        {
            Title = "High CPU Usage Detected",
            Description = "The server is experiencing high CPU usage, causing performance degradation.",
            Severity = "Sev 2",
            Status = "New",
            AssignedTo = "Team A",
            Link = "http://example.com/incident/1"
        },
        new Incident
        {
            Title = "Database Connection Timeout",
            Description = "The application is unable to connect to the database due to a timeout.",
            Severity = "Sev 1",
            Status = "New",
            AssignedTo = "Team B",
            Link = "http://example.com/incident/2"
        },
        new Incident
        {
            Title = "Application Crash on Startup",
            Description = "The application crashes immediately after startup.",
            Severity = "Sev 2.5",
            Status = "New",
            AssignedTo = "Team C",
            Link = "http://example.com/incident/3"
        },
        new Incident
        {
            Title = "Slow API Response",
            Description = "Users are reporting slow response times from the API.",
            Severity = "Sev 3",
            Status = "New",
            AssignedTo = "Team D",
            Link = "http://example.com/incident/4"
        },
        new Incident
        {
            Title = "Disk Space Full",
            Description = "The disk space on the server is full, causing application errors.",
            Severity = "Sev 1",
            Status = "New",
            AssignedTo = "Team E",
            Link = "http://example.com/incident/5"
        },
        // Random incident that does not map to any TSG
        new Incident
        {
            Title = "Unusual Network Traffic",
            Description = "The server is experiencing unusual network traffic, possibly indicating a DDoS attack.",
            Severity = "Sev 2",
            Status = "New",
            AssignedTo = "Team F",
            Link = "http://example.com/incident/6"
        }
    };

    public IncidentSimulationService(Agent agent)
    {
        _agent = agent;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Simulate adding a new incident
            var incident = _incidentTemplates[_random.Next(_incidentTemplates.Count)];
            incident.CreatedAt = DateTime.UtcNow; // Set the created time to now
            _agent.AddIncident(incident); // Add the incident to the agent's queue

            Console.WriteLine($"Simulated new incident: {incident.Title}");

            // Wait for a random interval before adding the next incident
            await Task.Delay(_random.Next(2, 1000), stoppingToken); // 2 ms - 1 second
        }
    }
}
