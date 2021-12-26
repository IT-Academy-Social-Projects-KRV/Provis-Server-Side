using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IConfirmEmailService
    {
        Task SendConfirmMailAsync(string userId);
    }
}
