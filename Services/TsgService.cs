using System.Text.Json;
using Microsoft.SemanticKernel.Memory;
using OncallAgent.Models;

namespace OncallAgent.Services;

public class TsgService
{
    private readonly ISemanticTextMemory _memory;
    private readonly string CollectionName = "TsgCollection";

    public TsgService(ISemanticTextMemory memory)
    {
        _memory = memory;
    }

    public async Task IndexTsgsAsync(string tsgDirectory)
    {
        var tsgFiles = Directory.GetFiles(tsgDirectory, "*.json", SearchOption.AllDirectories);

        foreach (var file in tsgFiles)
        {
            var tsg = await File.ReadAllTextAsync(file);
            var tsgModel = JsonSerializer.Deserialize<TsgModel>(tsg);

            if (tsgModel != null)
            {
                await _memory.SaveInformationAsync(
                    collection: CollectionName,
                    id: tsgModel.Id,
                    text: string.Join("\n", tsgModel.Steps),
                    description: tsgModel.Description);
                Console.WriteLine($"Indexed TSG: {tsgModel.Title}");
            }
        }
    }

    public async Task IndexSingleTsgAsync(string tsgFilePath)
    {
        var tsg = await File.ReadAllTextAsync(tsgFilePath);
        var tsgModel = JsonSerializer.Deserialize<TsgModel>(tsg);

        if (tsgModel != null)
        {
            await _memory.SaveInformationAsync(
                collection: CollectionName,
                id: tsgModel.Id,
                text: string.Join("\n", tsgModel.Steps),
                description: tsgModel.Description);
            Console.WriteLine($"Indexed TSG: {tsgModel.Title}");
        }
    }

    public async Task IndexSingleTsgAsync(TsgModel tsgModel)
    {
        if (tsgModel != null)
        {
            await _memory.SaveInformationAsync(
                collection: CollectionName,
                id: tsgModel.Id,
                text: string.Join("\n", tsgModel.Steps),
                description: tsgModel.Description);
            Console.WriteLine($"Indexed TSG: {tsgModel.Title}");
        }
    }

    public async Task<string?> RetrieveRelevantTsgAsync(string query)
    {
        await foreach (var result in _memory.SearchAsync(CollectionName, query, limit: 1, minRelevanceScore: 0.75))
        {
            Console.WriteLine($"Found relevant TSG: {result.Metadata.Id} with score: {result.Relevance}");
            return string.Join("\n", result?.Metadata.Description, result?.Metadata.Text); // Get only the first result
        }
        return null; // No relevant TSG found
    }
}
