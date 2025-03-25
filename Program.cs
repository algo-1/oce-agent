using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using OncallAgent.Models;
using OncallAgent.Services;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiKey = configuration["OpenAI:ApiKey"];

if (string.IsNullOrEmpty(openAiApiKey))
{
    Console.WriteLine("OpenAI API key is not set. Please set it in user secrets.");
    return;
}

var builder = Kernel.CreateBuilder();
builder.AddOpenAITextEmbeddingGeneration("text-embedding-ada-002", openAiApiKey);

var kernel = builder.Build();

// Set up Vector Database for retrieval
var memoryStore = new QdrantMemoryStore("http://localhost:6333", vectorSize: 1536);
var memory = new SemanticTextMemory(memoryStore, kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>());


Console.WriteLine("Press 'q' to quit or any other key to continue processing incidents...");

// Use Agent to process incidents
var agent = new Agent(maxWorkers: 5, memory: memory);
try
{
    // Start the agent
    agent.Start();

    // Simulate adding incidents to the queue
    for (int i = 0; i < 6; i++)
    {
        var incident = new Incident
        {
            Title = $"Incident {i + 1}",
            Description = GetRandomDescription(),
            Status = "New",
            Severity = GetRandomSeverity(),
            CreatedAt = DateTime.UtcNow,
            AssignedTo = "Agent",
            Tags = new List<string> { "tag1", "tag2" }
        };

        agent.AddIncident(incident);
    }

    while (true)
    {
        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.Q)
        {
            agent.Stop();
            break;
        }
    }
}
catch (Exception)
{
    Console.WriteLine("An error occurred while processing incidents.");
    // Stop the agent and clean up resources
    agent.Stop();
    Console.WriteLine("Agent stopped.");
}

string GetRandomDescription()
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

string GetRandomSeverity()
{
    var severities = new[] { "Critical", "High", "Medium", "Low" };
    var random = new Random();
    return severities[random.Next(severities.Length)];
}
