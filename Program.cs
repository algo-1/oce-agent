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


// // Sample incident
// while (true)
// {
//     Console.WriteLine("Please enter the incident description (or type 'exit' to quit):");
//     var incidentDescription = Console.ReadLine();

//     if (string.Equals(incidentDescription, "exit", StringComparison.OrdinalIgnoreCase))
//     {
//         break;
//     }

//     if (string.IsNullOrEmpty(incidentDescription))
//     {
//         Console.WriteLine("Incident description cannot be empty.");
//         continue;
//     }

//     var relevantTsg = await tsgService.RetrieveRelevantTsgAsync(incidentDescription);
//     if (relevantTsg is not null)
//     {
//         Console.WriteLine($"Relevant TSG found: {relevantTsg}");
//     }
//     else
//     {
//         Console.WriteLine("No relevant TSG found.");
//     }
// }

Console.WriteLine("Press 'q' to quit or any other key to continue processing incidents...");

// Use Agent to process incidents
var agent = new Agent(maxWorkers: 5, memory: memory);
try
{
    // Start the agent
    agent.Start();

    // Simulate adding incidents to the queue
    for (int i = 0; i < 10; i++)
    {
        var incident = new Incident
        {
            Title = $"Incident {i + 1}",
            Description = $"Description for incident {i + 1}",
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

string GetRandomSeverity()
{
    var severities = new[] { "Critical", "High", "Medium", "Low" };
    var random = new Random();
    return severities[random.Next(severities.Length)];
}
