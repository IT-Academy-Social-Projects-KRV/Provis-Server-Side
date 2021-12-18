using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task SendAsync(string emailAddress, string message);
    }
}
