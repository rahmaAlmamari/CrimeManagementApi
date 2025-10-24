using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrimeManagementApi.Services.Interfaces;
using CrimeManagementApi.Utilities;
using CrimeManagementApi.Mappings;

namespace CrimeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Investigator")]
    public class ReportController : ControllerBase
    {
        private readonly ICaseService _caseService;

        public ReportController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        /// <summary>
        /// Generates a professional PDF report for a specific case ID.
        /// </summary>
        [HttpGet("case/{id}/pdf")]
        public async Task<IActionResult> GenerateCasePdf(int id)
        {
            // Fetch the case from the service
            var caseDto = await _caseService.GetByIdAsync(id);
            if (caseDto == null)
                return NotFound("Case not found.");

            //  Convert DTO → Entity (so PdfGenerator can use navigation objects)
            var caseEntity = caseDto.ToEntity();

            //  Generate PDF
            var pdfBytes = PdfGenerator.GenerateCaseReport(caseEntity);

            //  Return as downloadable file
            return File(pdfBytes, "application/pdf", $"CaseReport_{caseEntity.CaseNumber}.pdf");
        }
    }
}
