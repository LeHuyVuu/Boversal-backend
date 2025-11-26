namespace UtilityService.Models;

public class EmailSettings
{
    public string SmtpServer { get; set; } = null!;
    public int Port { get; set; }
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Password { get; set; } = null!; // -> để ở User Secrets/ENV khi chạy thật
}