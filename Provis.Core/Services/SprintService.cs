using AutoMapper;
using Provis.Core.DTO.SprintDTO;
using Provis.Core.Entities.SprintEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Resources;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    class SprintService: ISprintService
    {
        private readonly IRepository<Workspace> _workspaceRepository;
        private readonly IRepository<WorkspaceTask> _taskRepository;
        private readonly IRepository<Sprint> _sprintRepository;
        private readonly IMapper _mapper;

        public SprintService(IRepository<Workspace> workspaceRepository,
            IRepository<WorkspaceTask> taskRepository,
            IRepository<Sprint> sprintRepository,
            IMapper mapper)
        {
            _workspaceRepository = workspaceRepository;
            _taskRepository = taskRepository;
            _sprintRepository = sprintRepository;
            _mapper = mapper;
        }

        public async Task<SprintInfoDTO> AddSprintAsync(ChangeSprintInfoDTO sprintInfo, int workspaceId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);

            if(!workspace.isUseSprints)
            {
                throw new HttpException(HttpStatusCode.BadRequest, ErrorMessages.SprintsNotOn);
            }

            var sprint = _mapper.Map<Sprint>(sprintInfo);
            sprint.WorkspaceId = workspaceId;

            sprint = await _sprintRepository.AddAsync(sprint);
            await _sprintRepository.SaveChangesAsync();

            return _mapper.Map<SprintInfoDTO>(sprint);
        }

        public async Task<SprintDetailInfoDTO> GetSprintById(int sprintId)
        {
            var sprint = await _sprintRepository.GetByKeyAsync(sprintId);
            sprint.SprintNullChecking();

            return _mapper.Map<SprintDetailInfoDTO>(sprint);
        }

        public async Task<List<SprintInfoDTO>> GetSprintListAsync(int workspaceId)
        {
            var specification = new Sptints.SprintInfo(workspaceId);
            var sprintList = await _sprintRepository.GetListBySpecAsync(specification);

            return _mapper.Map<List<SprintInfoDTO>>(sprintList);
        }

        public async Task OffSprints(int workspaceId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);

            if (!workspace.isUseSprints)
            {
                throw new HttpException(HttpStatusCode.BadRequest, ErrorMessages.SprintsAlreadyOff);
            }

            workspace.isUseSprints = false;
            await _workspaceRepository.SaveChangesAsync();

            await _taskRepository.SqlQuery($"UPDATE Tasks SET SprintId=NULL WHERE WorkspaceId={workspaceId}");

            await _taskRepository.SqlQuery($"DELETE FROM Sprints WHERE WorkspaceId={workspaceId}");

        }

        public async Task OnSprintsAsync(int workspaceId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);

            if(workspace.isUseSprints)
            {
                throw new HttpException(HttpStatusCode.BadRequest, ErrorMessages.SprintsAlreadyOn);
            }

            workspace.isUseSprints = true;

            var sprintToInsert = new Sprint()
            {
                Name = "Sprint name",
                WorkspaceId = workspaceId
            };

            var sprint = await _sprintRepository.AddAsync(sprintToInsert);
            await _sprintRepository.SaveChangesAsync();

            await _taskRepository.SqlQuery($"UPDATE Tasks SET SprintId={sprint.Id} WHERE WorkspaceId={workspaceId}");
        }

        public async Task UpdateSprintAsync(ChangeSprintInfoDTO sprintInfo, int sprintId)
        {
            var sprint = await _sprintRepository.GetByKeyAsync(sprintId);
            sprint.SprintNullChecking();

            _mapper.Map(sprintInfo, sprint);

            await _sprintRepository.UpdateAsync(sprint);
            await _sprintRepository.SaveChangesAsync();
        }
    }
}
