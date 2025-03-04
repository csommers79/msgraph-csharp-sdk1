﻿namespace DomainObjects.Models
{
    public class AppConfig
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;        
        public string EmailAddress { get; set; } = string.Empty;
    }
}
