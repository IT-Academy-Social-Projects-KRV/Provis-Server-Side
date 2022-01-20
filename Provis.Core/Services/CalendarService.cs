using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.EventDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.EventEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class CalendarService : ICalendarService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<UserWorkspace> _userWorkspaceRepository;
        protected readonly IRepository<Workspace> _workspaceRepository;
        protected readonly IRepository<WorkspaceTask> _taskRepository;
        protected readonly IRepository<Calendar> _calendarRepository;
        protected readonly IMapper _mapper;

        public CalendarService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Calendar> calendar,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<Workspace> workspace,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _workspaceRepository = workspace;
            _userWorkspaceRepository = userWorkspace;
            _userRepository = user;
            _taskRepository = task;
            _calendarRepository = calendar;
            _mapper = mapper;
        }

        public async Task<List<EventAllDTO>> GetAllEvents(int workspaceId, string userId)
        {
            var specification = new Calendars.GetAllEvents(userId, workspaceId);
            var eventList = await _calendarRepository.GetListBySpecAsync(specification);


        }
    }
}
