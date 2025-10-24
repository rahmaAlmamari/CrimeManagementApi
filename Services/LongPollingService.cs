using CrimeManagementApi.Data;
using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>
    /// Tracks and processes evidence hard-deletion using long-polling simulation.
    /// </summary>
    public class LongPollingService : ILongPollingService
    {
        private readonly AppDbContext _context;
        private static readonly Dictionary<int, DeletionStatus> _deletionTracker = new();

        public LongPollingService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Initiate deletion request by admin.</summary>
        public async Task<string> InitiateDeletionAsync(int evidenceId, int adminUserId)
        {
            var evidence = await _context.Evidence.FindAsync(evidenceId);
            if (evidence == null)
                return $"Evidence ID {evidenceId} not found.";

            _deletionTracker[evidenceId] = new DeletionStatus
            {
                EvidenceId = evidenceId,
                Status = "Pending",
                UpdatedAt = DateTime.UtcNow
            };

            return $"Are you sure you want to permanently delete Evidence ID: {evidenceId}? (yes/no)";
        }

        /// <summary>Confirm deletion after admin response.</summary>
        public async Task<string> ConfirmDeletionAsync(int evidenceId, int adminUserId, string confirmation)
        {
            try
            {
                if (!_deletionTracker.ContainsKey(evidenceId))
                    return $"No active deletion request for Evidence ID {evidenceId}.";

                if (confirmation.ToLower() != "yes")
                {
                    _deletionTracker[evidenceId].Status = "Canceled";
                    return "Deletion canceled by admin.";
                }

                _deletionTracker[evidenceId].Status = "InProgress";

                await Task.Delay(2000); // simulate background deletion

                var entity = await _context.Evidence.FirstOrDefaultAsync(e => e.Id == evidenceId);
                if (entity == null)
                {
                    _deletionTracker[evidenceId].Status = "Failed";
                    return $"Evidence ID {evidenceId} not found.";
                }

                _context.Evidence.Remove(entity);
                await _context.SaveChangesAsync();

                _deletionTracker[evidenceId].Status = "Completed";
                _deletionTracker[evidenceId].UpdatedAt = DateTime.UtcNow;

                return $"Evidence ID {evidenceId} permanently deleted.";
            }
            catch (Exception ex)
            {
                _deletionTracker[evidenceId].Status = "Failed";
                _deletionTracker[evidenceId].UpdatedAt = DateTime.UtcNow;
                return $" Error deleting evidence ID {evidenceId}: {ex.Message}";
            }
        }


        /// <summary>Return live deletion progress (long-poll style).</summary>
        public async Task<DeletionStatus> GetDeletionStatusAsync(int evidenceId)
        {
            for (int i = 0; i < 10; i++) // simulate long-poll waiting
            {
                if (_deletionTracker.TryGetValue(evidenceId, out var status) &&
                    (status.Status == "Completed" || status.Status == "Failed"))
                    return status;

                await Task.Delay(1000);
            }

            return _deletionTracker.ContainsKey(evidenceId)
                ? _deletionTracker[evidenceId]
                : new DeletionStatus { EvidenceId = evidenceId, Status = "Unknown" };
        }
    }
}
