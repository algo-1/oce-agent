using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;

var builder = Kernel.CreateBuilder();
builder.AddOpenAITextEmbeddingGeneration("text-embedding-ada-002", "YOUR_OPENAI_API_KEY");

var kernel = builder.Build();

// Set up Vector Database for retrieval

var memoryStore = new QdrantMemoryStore("http://localhost:6333", vectorSize: 1536);

var memory = new SemanticTextMemory(memoryStore, kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>());


