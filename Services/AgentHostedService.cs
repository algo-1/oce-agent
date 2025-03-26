using Microsoft.Extensions.Hosting;

namespace OncallAgent.Services;

public class AgentHostedService : IHostedService
{
    private readonly Agent _agent;

    public AgentHostedService(Agent agent)
    {
        _agent = agent;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting Agent...");
        _agent.Start(); // Start the Agent
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping Agent...");
        _agent.Stop(); // Stop the Agent
        return Task.CompletedTask;
    }
}
