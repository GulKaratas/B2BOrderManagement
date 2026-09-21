namespace B2BOrderManagement.Api.Domain.Entities;

public class IntegrationLog
{
    public int Id { get; set; }
    public string Direction { get; set; } = "Inbound";
    public string EventType { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public string Payload { get; set; } = string.Empty;
    public IntegrationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
