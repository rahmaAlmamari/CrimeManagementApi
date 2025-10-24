using CrimeManagementApi.DTOs;

namespace CrimeManagementApi.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetByCaseIdAsync(int caseId);
        Task<CommentDto?> CreateAsync(CreateCommentDto dto);
        Task<CommentDto?> UpdateAsync(int id, UpdateCommentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
