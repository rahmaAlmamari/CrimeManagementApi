using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using System.Linq;

namespace CrimeManagementApi.Mappings
{
    /// <summary>
    /// Extension methods for converting DTOs to Entity models (for PDF generation and reports).
    /// </summary>
    public static class DtoToEntityExtensions
    {
        /// <summary>
        /// Converts CaseDto to Case entity for PDF generation.
        /// Includes created user, participants, and evidence mapping.
        /// </summary>
        public static Case ToEntity(this CaseDto dto)
        {
            return new Case
            {
                Id = dto.Id,
                CaseNumber = dto.CaseNumber,
                Name = dto.Name,
                Description = dto.Description,
                CaseType = dto.CaseType,
                ClearanceLevel = dto.ClearanceLevel,
                Status = dto.Status,
                AreaCity = dto.AreaCity,
                CreatedAt = dto.CreatedAt,

                // 🔹 Creator mapping
                CreatedByUser = dto.CreatedByUserId > 0
                    ? new User
                    {
                        Id = dto.CreatedByUserId,
                        FullName = dto.CreatedByUserName ?? "N/A",
                        Role = dto.CreatedByUserRole ?? "Unknown"
                    }
                    : null,

                // 🔹 Evidence mapping (updated to match model)
                EvidenceList = dto.Evidences?.Select(e => new Evidence
                {
                    Id = e.Id,
                    Type = e.Type,
                    Description = e.Description ?? "-",
                    FilePath = e.FilePath ?? "-",
                    MimeType = e.MimeType ?? "-",
                    Remarks = e.Remarks ?? "-",
                    AddedAt = e.AddedAt
                }).ToList()
            };
        }
    }
}
