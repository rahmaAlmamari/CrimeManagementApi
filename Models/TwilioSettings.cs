﻿namespace CrimeManagementApi.Models
{
    public class TwilioSettings
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}