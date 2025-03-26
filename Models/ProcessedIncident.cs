namespace OncallAgent.Models;

public class ProcessedIncident
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Severity { get; set; }
    public string? Tsg { get; set; } = null;
    public required string Link { get; set; }

    public override string ToString()
    {
        return $"Incident ID: {Id}, Title: {Title}, Status: {Status}, Severity: {Severity}";
    }
}
