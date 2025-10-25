using CrimeManagementApi.Models;

namespace CrimeManagementApi.Services.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string toPhoneNumber, string message);
        Task SendCaseUpdateSmsAsync(string toPhoneNumber, string caseNumber, string newStatus);
        Task SendNewCaseAssignmentSmsAsync(string toPhoneNumber, string caseNumber, string assignedTo);
        Task SendUrgentAlertSmsAsync(string toPhoneNumber, string alertMessage);
    }
}