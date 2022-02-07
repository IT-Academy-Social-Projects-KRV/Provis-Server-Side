using Provis.Core.DTO.SprintDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ISprintService
    {
        Task OnSprintsAsync(int workspaceId);
        Task OffSprints(int workspaceId);
        Task<List<SprintInfoDTO>> GetSprintListAsync(int workspaceId);
        Task<SprintDetailInfoDTO> GetSprintById(int sprintId);
        Task<SprintInfoDTO> AddSprintAsync(ChangeSprintInfoDTO sprintInfo, int workspaceId);
        Task UpdateSprintAsync(ChangeSprintInfoDTO sprintInfo, int sprintId);
    }
}
