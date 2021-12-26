using Provis.Core.DTO.userDTO;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Interfaces.Services
{
    public interface IConfirmEmailService
    {
        Task SendConfirmMailAsync(string userId);

        Task ConfirmEmailAsync(string userId, UserConfirmEmailDTO confirmEmailDTO);
    }
}
