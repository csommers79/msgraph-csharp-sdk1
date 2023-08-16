namespace DomainObjects.Models
{
    public class AppConfig
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string LocalSourceFolder { get; set; } = string.Empty;
        public string OneDriveTargetFolder { get; set; } = string.Empty;

        public string OneDriveSourceFolder { get; set; } = string.Empty;
        public string LocalTargetFolder { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public bool EnableLog { get; set; }
        public string LogFolderPath { get; set; } = string.Empty;
    }
}
