namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>
    /// Defines operations for sending emails related to system alerts and updates.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>Send a general-purpose email message.</summary>
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        /// <summary>Notify a user about a case status update.</summary>
        Task SendCaseUpdateAsync(string to, string caseNumber, string newStatus);

        /// <summary>Send a crime awareness alert to citizens in a specific area.</summary>
        Task SendNewCrimeAlertAsync(string to, string area, string description);
    }
}
