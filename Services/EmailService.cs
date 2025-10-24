using CrimeManagementApi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CrimeManagementApi.Services.Implementations
{
    /// <summary>
    /// Handles all email delivery operations for system notifications.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;

        public EmailService(IConfiguration config)
        {
            _config = config;
            Console.WriteLine(" Loaded Email Config:");
            Console.WriteLine($"Server: {_config["EmailSettings:SmtpServer"]}");
            Console.WriteLine($"Port: {_config["EmailSettings:SmtpPort"]}");
            Console.WriteLine($"User: {_config["EmailSettings:Username"]}");

            _smtpServer = _config["EmailSettings:SmtpServer"] ?? throw new ArgumentNullException("SmtpServer");
            _smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? throw new ArgumentNullException("SmtpPort"));
            _smtpUser = _config["EmailSettings:Username"] ?? throw new ArgumentNullException("Username");
            _smtpPass = _config["EmailSettings:Password"] ?? throw new ArgumentNullException("Password");
            _fromEmail = _config["EmailSettings:FromEmail"] ?? throw new ArgumentNullException("FromEmail");
        }


        // ======================================================
        // 🔹 1. General-purpose email sender
        // ======================================================
        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            var message = new MailMessage(_fromEmail, to, subject, body)
            {
                IsBodyHtml = isHtml
            };

            await client.SendMailAsync(message);
            Console.WriteLine($"📧 Email sent to {to} with subject '{subject}'");
        }

        // ======================================================
        // 🔹 2. Case Status Update Notification
        // ======================================================
        public async Task SendCaseUpdateAsync(string to, string caseNumber, string newStatus)
        {
            string subject = $"Case #{caseNumber} – Status Update";
            string body = $@"
                <h3>Case Update Notification</h3>
                <p>Your case <b>#{caseNumber}</b> has been updated to status:
                <span style='color:blue;font-weight:bold;'>{newStatus}</span>.</p>
                <p>Thank you for using the Crime Management System.</p>";

            await SendEmailAsync(to, subject, body);
        }

        // ======================================================
        // 🔹 3. New Crime Alert Broadcast
        // ======================================================
        public async Task SendNewCrimeAlertAsync(string to, string area, string description)
        {
            string subject = $"🚨 New Crime Alert in {area}";
            string body = $@"
                <h3>Crime Alert – Stay Safe!</h3>
                <p>A new crime has been reported in your area: <b>{area}</b>.</p>
                <p><b>Description:</b> {description}</p>
                <p>Stay alert and contact authorities if you have relevant information.</p>
                <br>
                <small>Crime Management API – Automated Alert</small>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
