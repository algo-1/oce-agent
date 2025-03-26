using Microsoft.Extensions.Hosting;
using OncallAgent.Models;

namespace OncallAgent.Services;

public class IncidentSimulationService : BackgroundService
{
    private readonly Agent _agent;

    public IncidentSimulationService(Agent agent)
    {
        _agent = agent;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            // Simulate adding a new incident
            var number = random.Next(1, 1000);
            var incident = new Incident
            {
                Title = $"Incident {number}",
                Description = GetRandomDescription(),
                Status = "New",
                Severity = GetRandomSeverity(),
                CreatedAt = DateTime.UtcNow,
                AssignedTo = "Agent",
                Tags = new List<string> { "tag1", "tag2" },
                Link = $"http://example.com/incident/{number}"
            };

            _agent.AddIncident(incident); // Add the incident to the agent's queue
            Console.WriteLine($"Simulated new incident: {incident.Title}");

            // Wait for a random interval before adding the next incident
            await Task.Delay(random.Next(2, 1000), stoppingToken); // 2 ms - 1 second
        }
    }

    private string GetRandomDescription()
    {
        var descriptions = new[]
        {
            "Database connection error",
            "API response timeout",
            "User forgot password",
            "Service unavailable",
            "Application not found"
        };
        var random = new Random();
        return descriptions[random.Next(descriptions.Length)];
    }

    private string GetRandomSeverity()
    {
        var severities = new[] { "Sev 1", "Sev 2", "Sev 2.5", "Sev 3", "Sev 4" };
        var random = new Random();
        return severities[random.Next(severities.Length)];
    }
}
