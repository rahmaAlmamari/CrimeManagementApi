using System.ComponentModel.DataAnnotations;

namespace CrimeManagementApi.DTOs
{
    // ============================================================
    // 🔹 EmailSettingsDto.cs — DTOs for email configuration & sending
    // ============================================================

    /// <summary>
    /// Holds SMTP configuration settings for sending email notifications.
    /// These values can be loaded from appsettings.json or environment variables.
    /// </summary>
    public class EmailSettingsDto
    {
        /// <summary>SMTP server hostname (e.g., smtp.gmail.com).</summary>
        [Required, MaxLength(100)]
        public string SmtpServer { get; set; } = string.Empty;

        /// <summary>Port number for the SMTP server (commonly 465 or 587).</summary>
        [Range(1, 65535)]
        public int Port { get; set; }

        /// <summary>Display name for the sender (e.g., “District Core Crime System”).</summary>
        [Required, MaxLength(100)]
        public string SenderName { get; set; } = string.Empty;

        /// <summary>Email address used to send messages.</summary>
        [Required, EmailAddress]
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>Password or app-specific token for authentication.</summary>
        [Required, MaxLength(200)]
        public string Password { get; set; } = string.Empty;

        /// <summary>Enables SSL or TLS encryption for secure communication.</summary>
        public bool EnableSsl { get; set; } = true;
    }

    // ============================================================
    // 🔹 EmailMessageDto.cs — Represents the structure of an email
    // ============================================================

    /// <summary>
    /// Represents an email message being sent by the system.
    /// Used by the EmailService to construct and dispatch messages.
    /// </summary>
    public class EmailMessageDto
    {
        /// <summary>Recipient email address.</summary>
        [Required, EmailAddress]
        public string To { get; set; } = string.Empty;

        /// <summary>Email subject line.</summary>
        [Required, MaxLength(150)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>Main content or HTML body of the email.</summary>
        [Required]
        public string Body { get; set; } = string.Empty;

        /// <summary>Optional flag to indicate HTML formatting.</summary>
        public bool IsHtml { get; set; } = true;

        /// <summary>Optional CC recipient(s).</summary>
        public string? Cc { get; set; }

        /// <summary>Optional BCC recipient(s).</summary>
        public string? Bcc { get; set; }
    }

    // ============================================================
    // 🔹 EmailNotificationDto.cs — High-level notification trigger
    // ============================================================

    /// <summary>
    /// Represents structured data for triggering predefined email notifications,
    /// such as crime alerts, evidence updates, or safety messages.
    /// </summary>
    public class EmailNotificationDto
    {
        /// <summary>Email address of the recipient.</summary>
        [Required, EmailAddress]
        public string RecipientEmail { get; set; } = string.Empty;

        /// <summary>Type of notification (CaseUpdate, NewReport, SafetyAlert, etc.).</summary>
        [Required, MaxLength(50)]
        public string NotificationType { get; set; } = string.Empty;

        /// <summary>Optional reference ID (e.g., CaseId, ReportId, EvidenceId).</summary>
        public int? ReferenceId { get; set; }

        /// <summary>Dynamic content or variables for the notification template.</summary>
        [MaxLength(2000)]
        public string? MessageBody { get; set; }

        /// <summary>UTC timestamp when the notification was generated.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
