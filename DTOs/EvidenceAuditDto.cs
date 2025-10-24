namespace CrimeManagementApi.DTOs
{
    public class EvidenceAuditDto
    {
        public int Id { get; set; }
        public int EvidenceId { get; set; }
        public int? ActedByUserId { get; set; }

        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime ActedAt { get; set; }

        // Optional for frontend display
        public string? ActedByUserName { get; set; }
    }
}
