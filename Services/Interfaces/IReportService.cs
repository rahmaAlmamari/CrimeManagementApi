namespace CrimeManagementApi.Services.Interfaces
{
    /// <summary>Generate PDF reports for cases.</summary>
    public interface IReportService
    {
        Task<byte[]> GenerateCaseReportAsync(int caseId);
    }
}
