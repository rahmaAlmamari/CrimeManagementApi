using AutoMapper;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>
    /// Handles all business logic for case comments.
    /// Supports add, update, soft delete, and retrieval.
    /// </summary>
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CommentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ==========================================================
        // 🔹 Get all comments for a specific case (ordered by date)
        // ==========================================================
        public async Task<IEnumerable<CommentDto>> GetByCaseIdAsync(int caseId)
        {
            var comments = await _context.CaseComments
                .Where(c => c.CaseId == caseId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        // ==========================================================
        // 🔹 Add a new comment to a case
        // ==========================================================
        public async Task<CommentDto?> CreateAsync(CreateCommentDto dto)
        {
            var comment = _mapper.Map<CaseComment>(dto);
            comment.CreatedAt = DateTime.UtcNow;

            _context.CaseComments.Add(comment);
            await _context.SaveChangesAsync();

            return _mapper.Map<CommentDto>(comment);
        }

        // ==========================================================
        // 🔹 Update comment content or pin/delete flags
        // ==========================================================
        public async Task<CommentDto?> UpdateAsync(int id, UpdateCommentDto dto)
        {
            var comment = await _context.CaseComments.FindAsync(id);
            if (comment == null || comment.IsDeleted)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Content))
                comment.Content = dto.Content;

            if (dto.IsPinned.HasValue)
                comment.IsPinned = dto.IsPinned.Value;

            if (dto.IsDeleted.HasValue)
                comment.IsDeleted = dto.IsDeleted.Value;

            comment.UpdatedAt = dto.UpdatedAt ?? DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<CommentDto>(comment);
        }

        // ==========================================================
        // 🔹 Soft delete a comment
        // ==========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var comment = await _context.CaseComments.FindAsync(id);
            if (comment == null)
                return false;

            comment.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
