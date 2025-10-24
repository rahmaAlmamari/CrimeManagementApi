using CrimeManagementApi.Models;

namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines long-polling operations for tracking evidence hard-deletion progress.
    /// </summary>
    public interface ILongPollingService
    {
        /// <summary>
        /// Initiates the hard-deletion process for a specific evidence record.
        /// </summary>
        Task<string> InitiateDeletionAsync(int evidenceId, int adminUserId);

        /// <summary>
        /// Confirms or cancels the deletion based on admin response ("yes" or "no").
        /// </summary>
        Task<string> ConfirmDeletionAsync(int evidenceId, int adminUserId, string confirmation);

        /// <summary>
        /// Returns the current status of an ongoing or completed deletion.
        /// </summary>
        Task<DeletionStatus> GetDeletionStatusAsync(int evidenceId);
    }
}
