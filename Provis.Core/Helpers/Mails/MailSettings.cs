using MimeKit.Text;

namespace Provis.Core.Helpers.Mails
{
    public class MailSettings
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public TextFormat TextFormat { get; set; }
    }
}
