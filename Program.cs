using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using OncallAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiKey = configuration["OpenAI:ApiKey"];

if (string.IsNullOrEmpty(openAiApiKey))
{
    Console.WriteLine("OpenAI API key is not set. Please set it in user secrets.");
    return;
}

// Set up Semantic Kernel and Memory
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAITextEmbeddingGeneration("text-embedding-ada-002", openAiApiKey);

var kernel = kernelBuilder.Build();

// Set up Vector Database for retrieval
var memoryStore = new QdrantMemoryStore("http://localhost:6333", vectorSize: 1536);
var memory = new SemanticTextMemory(memoryStore, kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>());

// Register services in the DI container
builder.Services.AddSingleton<ISemanticTextMemory>(memory); // Register memory
builder.Services.AddSingleton<Agent>(sp =>
{
    var memory = sp.GetRequiredService<ISemanticTextMemory>();
    return new Agent(maxWorkers: 5, memory: memory);
});
builder.Services.AddHostedService<AgentHostedService>();
builder.Services.AddHostedService<ProcessedIncidentQueueService>(); // Background service for incident queue
builder.Services.AddHostedService<IncidentSimulationService>(); // Background service for simulating incidents
builder.Services.AddSignalR(); // SignalR for real-time updates
builder.Services.AddControllers(); // API controllers

var app = builder.Build();

// Configure middleware
app.UseRouting();

// Map API controllers and SignalR hub
app.MapControllers(); // Map API controllers
app.MapHub<IncidentHub>("/incidentHub"); // Map SignalR hub

// Start the application
app.Run();
