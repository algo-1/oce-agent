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

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register services in the DI container
builder.Services.AddSingleton<ISemanticTextMemory>(memory); // Register memory
builder.Services.AddSingleton<TsgService>(); // Register TsgService
builder.Services.AddSingleton<Agent>(sp =>
{
    var memory = sp.GetRequiredService<ISemanticTextMemory>();
    var tsgService = sp.GetRequiredService<TsgService>();
    return new Agent(maxWorkers: 5, memory: memory, tsgService: tsgService);
});
builder.Services.AddHostedService<AgentHostedService>();
builder.Services.AddHostedService<ProcessedIncidentQueueService>(); // Background service for incident queue
builder.Services.AddHostedService<IncidentSimulationService>(); // Background service for simulating incidents
builder.Services.AddSignalR(); // SignalR for real-time updates
builder.Services.AddControllers(); // API controllers

var app = builder.Build();

// Configure middleware
app.UseRouting();
app.UseCors("AllowFrontend");

// Map API controllers and SignalR hub
app.MapControllers(); // Map API controllers
app.MapHub<IncidentHub>("/incidentHub"); // Map SignalR hub

// Start the application
app.Run();
