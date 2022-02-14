using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.Entities.EventEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserEventsEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
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
        protected readonly IRepository<Event> _eventRepository;
        protected readonly IRepository<UserTask> _userTaskRepository;
        protected readonly IRepository<UserEvent> _userEventRepository;
        protected readonly IMapper _mapper;

        public CalendarService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Event> @event,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<Workspace> workspace,
            UserManager<User> userManager,
            IRepository<UserTask> userTask,
            IRepository<UserEvent> userEvent,
            IMapper mapper)
        {
            _userManager = userManager;
            _workspaceRepository = workspace;
            _userWorkspaceRepository = userWorkspace;
            _userRepository = user;
            _taskRepository = task;
            _eventRepository = @event;
            _mapper = mapper;
            _userTaskRepository = userTask;
            _userEventRepository = userEvent;
        }

        public async Task CreateEventAsync(EventCreateDTO eventCreateDTO, string userId)
        {
            if (eventCreateDTO.DateOfStart > eventCreateDTO.DateOfEnd)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest,
                        ErrorMessages.InvalidDateOfEnd);
            }

            var workspace = await _workspaceRepository.GetByKeyAsync(eventCreateDTO.WorkspaceId);
            workspace.WorkspaceNullChecking();

            var workspaceEvent = new Event()
            {
                CreatorId = userId,
                IsCreatorExist = false
            };
            _mapper.Map(eventCreateDTO, workspaceEvent);

            await _eventRepository.AddAsync(workspaceEvent);

            List<UserEvent> userEvents = new();
            foreach (var item in eventCreateDTO.AssignedUsers)
            {
                var user = await _userManager.FindByIdAsync(item.UserId);
                user.UserNullChecking();

                if (userEvents.Exists(x => x.UserId == item.UserId))
                {
                    throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                        ErrorMessages.UserAlreadyHasInvite);
                }
                userEvents.Add(new UserEvent
                {
                    EventId = workspaceEvent.Id,
                    UserId = item.UserId
                });
                if (item.UserId == userId)
                {
                    workspaceEvent.IsCreatorExist = true;
                }
            }

            await _eventRepository.SaveChangesAsync();

            await _userEventRepository.AddRangeAsync(userEvents);
            await _userEventRepository.SaveChangesAsync();
        }

        public async Task<List<EventDTO>> GetAllEventsAsync(int workspaceId, string userId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);
            workspace.WorkspaceNullChecking();

            var eventSpecification = new Events.GetMyEvents(userId, workspaceId);
            var eventList = await _eventRepository.GetListBySpecAsync(eventSpecification);

            var userEventSpecification = new UserEvents.GetMyEvents(userId, workspaceId);
            var userEventList = await _userEventRepository.GetListBySpecAsync(userEventSpecification);

            var userSpecification = new UserWorkspaces.UsersDateOfBirth(workspaceId);
            var userBirthdayList = await _userWorkspaceRepository.GetListBySpecAsync(userSpecification);

            var userWorkspSpecification = new UserWorkspaces.WorkspaceOwnerManager(userId, workspaceId);
            var isOwnerOrManager = await _userWorkspaceRepository.AnyBySpecAsync(userWorkspSpecification);

            if (isOwnerOrManager)
            {
                var allWorkspaceTasks = new WorkspaceTasks.AllWorkspaceTasks(workspaceId);
                var allTasksList = await _taskRepository.GetListBySpecAsync(allWorkspaceTasks);

                var listToReturn = allTasksList.ToList();
                listToReturn.AddRange(userBirthdayList);
                listToReturn.AddRange(eventList);
                listToReturn.AddRange(userEventList);

                return listToReturn;
            }
            else
            {
                var taskSpecification = new WorkspaceTasks.TaskByUser(userId, workspaceId);
                var taskList = await _taskRepository.GetListBySpecAsync(taskSpecification);

                var userTask = new UserTasks.TaskByUser(userId, workspaceId);
                var userTaskList = await _userTaskRepository.GetListBySpecAsync(userTask);

                var listToReturn = taskList.ToList();
                listToReturn.AddRange(userTaskList);
                listToReturn.AddRange(userBirthdayList);
                listToReturn.AddRange(eventList);
                listToReturn.AddRange(userEventList);

                return listToReturn;
            }
        }

        public async Task<List<EventDayDTO>> GetDayEventsAsync(int workspaceId, DateTimeOffset dateTime, string userId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);
            workspace.WorkspaceNullChecking();

            var eventSpecification = new Events.GetDayEvents(userId, workspaceId, dateTime);
            var eventList = await _eventRepository.GetListBySpecAsync(eventSpecification);

            var userEventsDay = new UserEvents.GetDayEvents(userId, workspaceId, dateTime);
            var userEventList = await _userEventRepository.GetListBySpecAsync(userEventsDay);

            var userSpecification = new UserWorkspaces.UsersDateOfBirthDetail(workspaceId, dateTime);
            var userBirthdayList = await _userWorkspaceRepository.GetListBySpecAsync(userSpecification);

            var userWorkspSpecification = new UserWorkspaces.WorkspaceOwnerManager(userId, workspaceId);
            var isOwnerOrManager = await _userWorkspaceRepository.AnyBySpecAsync(userWorkspSpecification);

            if (isOwnerOrManager)
            {
                var allWorkspaceTasks = new WorkspaceTasks.AllWorkspaceDayTasks(workspaceId, dateTime);
                var allTasksList = await _taskRepository.GetListBySpecAsync(allWorkspaceTasks);

                var listToReturn = eventList.ToList();
                listToReturn.AddRange(userEventList);
                listToReturn.AddRange(userBirthdayList);
                listToReturn.AddRange(allTasksList);

                return listToReturn;
            }
            else
            {
                var taskSpecification = new WorkspaceTasks.TaskDayByUser(userId, workspaceId, dateTime);
                var taskList = await _taskRepository.GetListBySpecAsync(taskSpecification);

                var userTask = new UserTasks.TaskDayByUser(userId, workspaceId, dateTime);
                var userTaskList = await _userTaskRepository.GetListBySpecAsync(userTask);

                var listToReturn = eventList.ToList();
                listToReturn.AddRange(userEventList);
                listToReturn.AddRange(userBirthdayList);
                listToReturn.AddRange(taskList);
                listToReturn.AddRange(userTaskList);

                return listToReturn;
            }
        }
    }
}
