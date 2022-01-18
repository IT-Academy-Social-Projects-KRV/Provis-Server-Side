using System.Collections.Generic;

namespace Provis.Core.Helpers.Mails
{
    public class MailingRequest<T>
    {
        public IEnumerable<string> Emails { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public T ViewModel { get; set; }
    }
}
