using CrimeManagementApi.Models;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CrimeManagementApi.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly TwilioSettings _twilioSettings;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(IOptions<TwilioSettings> twilioSettings, ILogger<TwilioSmsService> logger)
        {
            _twilioSettings = twilioSettings.Value;
            _logger = logger;

            // Initialize Twilio client
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
        }

        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_twilioSettings.PhoneNumber),
                    to: new PhoneNumber(toPhoneNumber)
                );

                _logger.LogInformation($"SMS sent to {toPhoneNumber}. SID: {messageResource.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS to {toPhoneNumber}");
                return false;
            }
        }

        public async Task SendCaseUpdateSmsAsync(string toPhoneNumber, string caseNumber, string newStatus)
        {
            string message = $"🔔 Case #{caseNumber} Update\nStatus changed to: {newStatus}\n-Crime Management System";
            await SendSmsAsync(toPhoneNumber, message);
        }

        public async Task SendNewCaseAssignmentSmsAsync(string toPhoneNumber, string caseNumber, string assignedTo)
        {
            string message = $"🆕 New Case Assignment\nCase: #{caseNumber}\nAssigned to: {assignedTo}\nPlease check your dashboard.\n-Crime Management System";
            await SendSmsAsync(toPhoneNumber, message);
        }

        public async Task SendUrgentAlertSmsAsync(string toPhoneNumber, string alertMessage)
        {
            string message = $"🚨 URGENT ALERT\n{alertMessage}\n-Crime Management System";
            await SendSmsAsync(toPhoneNumber, message);
        }
    }
}