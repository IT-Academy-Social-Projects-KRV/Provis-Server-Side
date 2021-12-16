using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IEmailSenderService
    {
        void Send(string emailAddress, string message, string userName);
    }
}
