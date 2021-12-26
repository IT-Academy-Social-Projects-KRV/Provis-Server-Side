using Provis.Core.Entities;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IConfirmEmailService
    {
        Task SendConfirmMailAsync(User user);
    }
}
