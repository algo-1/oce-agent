using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
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

// Index TSGs
var tsgService = new TsgService(memory);
await tsgService.IndexTsgsAsync("./Data/Tsgs");

// Sample incident
var incidentDescription = "User unable to access the application. Error message: 'Application not found'.";
var relevantTsg = await tsgService.RetrieveRelevantTsgAsync(incidentDescription);
if (relevantTsg is not null)
{
    Console.WriteLine($"Relevant TSG found: {relevantTsg}");
}
else
{
    Console.WriteLine("No relevant TSG found.");
}

