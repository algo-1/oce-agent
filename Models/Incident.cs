namespace OncallAgent.Models;

public class Incident
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Severity { get; set; }
    public required string AssignedTo { get; set; }
    public List<string> Tags { get; set; } = new();

    public override string ToString()
    {
        return $"Incident ID: {Id}, Title: {Title}, Status: {Status}, Severity: {Severity}, Assigned To: {AssignedTo}";
    }
}
