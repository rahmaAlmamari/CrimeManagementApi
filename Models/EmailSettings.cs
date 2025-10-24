using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.Models
{
    /// <summary>
    /// Represents configuration for SMTP-based email sending.
    /// Typically bound from the "EmailSettings" section of appsettings.json.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// SMTP host address (e.g., smtp.gmail.com, smtp.office365.com).
        /// </summary>
        [Required, MaxLength(150)]
        public string SmtpServer { get; set; } = string.Empty;

        /// <summary>
        /// Port number used for the SMTP connection (e.g., 587 or 465).
        /// </summary>
        [Range(1, 65535)]
        public int Port { get; set; }

        /// <summary>
        /// Display name that appears in outgoing email messages.
        /// </summary>
        [Required, MaxLength(100)]
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// The email address from which messages are sent.
        /// </summary>
        [Required, EmailAddress]
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>
        /// Password or app-specific key for SMTP authentication.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Whether SSL/TLS encryption should be enabled for the SMTP connection.
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Optional flag to enable or disable email notifications globally.
        /// </summary>
        public bool EnableEmailNotifications { get; set; } = true;
    }
}
