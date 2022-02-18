using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.DTO.UserDTO;
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
using Provis.Core.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            IRepository<Event> eventService,
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
            _eventRepository = eventService;
            _mapper = mapper;
            _userTaskRepository = userTask;
            _userEventRepository = userEvent;
        }

        public async Task CreateEventAsync(EventCreateDTO eventCreateDTO, string userId)
        {
            if (eventCreateDTO.DateOfStart > eventCreateDTO.DateOfEnd)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                        ErrorMessages.InvalidDateOfEnd);
            }

            var workspace = await _workspaceRepository.GetByKeyAsync(eventCreateDTO.WorkspaceId);
            workspace.WorkspaceNullChecking();

            foreach (var item in eventCreateDTO.AssignedUsers)
            {
                var specification = new UserWorkspaces.WorkspaceMember(item.UserId, workspace.Id);
                var userWorkspace = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);
                userWorkspace.UserWorkspaceNullChecking();
            }    

            var workspaceEvent = new Event()
            {
                CreatorId = userId,
                IsCreatorExist = false
            };
            _mapper.Map(eventCreateDTO, workspaceEvent);

            using var transaction = await _eventRepository.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);
            try
            {
                await _eventRepository.AddAsync(workspaceEvent);
                await _eventRepository.SaveChangesAsync();

                List<UserEvent> userEvents = new();
                foreach (var item in eventCreateDTO.AssignedUsers)
                {
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

                await _userEventRepository.AddRangeAsync(userEvents);
                await _userEventRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.TransactionFailed);
            }
        }

        public async Task EditEventAsync(EventEditDTO eventEditDTO, string userId)
        {
            var changeEvent = await _eventRepository.GetByKeyAsync(eventEditDTO.EventId);
            changeEvent.EventNullChecking();

            bool notCreator = changeEvent.CreatorId != userId;
            if (notCreator)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                        ErrorMessages.NotPermission);
            }

            bool wrongDate = eventEditDTO.DateOfStart > eventEditDTO.DateOfEnd;
            if (wrongDate)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                        ErrorMessages.InvalidDateOfEnd);
            }

            _mapper.Map(eventEditDTO, changeEvent);

            await _eventRepository.UpdateAsync(changeEvent);
            await _eventRepository.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(int eventId, string userId)
        {
            var deleteEvent = await _eventRepository.GetByKeyAsync(eventId);
            deleteEvent.EventNullChecking();

            bool notCreator = deleteEvent.CreatorId != userId;
            if (notCreator)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                        ErrorMessages.NotPermission);
            }

            await _eventRepository.DeleteAsync(deleteEvent);
            await _eventRepository.SaveChangesAsync();
        }

        public async Task LeaveEventAsync(int eventId, string userId, int workspaceId)
        {
            var eventLeave = await _eventRepository.GetByKeyAsync(eventId);
            eventLeave.EventNullChecking();

            var userEvent = await _userEventRepository.GetByPairOfKeysAsync(userId, eventId);
            userEvent.UserEventNullChecking();

            var userWorkspace = await _userWorkspaceRepository.GetByPairOfKeysAsync(userId, workspaceId);
            userWorkspace.UserWorkspaceNullChecking();

            bool EventCreatorOrNotWorkspaceOwnerOrManager = eventLeave.CreatorId == userId
                || !(userWorkspace.RoleId == (int)WorkSpaceRoles.OwnerId)
                && !(userWorkspace.RoleId == (int)WorkSpaceRoles.ManagerId);

            if (EventCreatorOrNotWorkspaceOwnerOrManager)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                        ErrorMessages.NotPermission);
            }

            await _userEventRepository.DeleteAsync(userEvent);
            await _userEventRepository.SaveChangesAsync();
        }

        public async Task<List<EventDTO>> GetAllEventsAsync(int workspaceId, string userId)
        {
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

        public async Task<EventGetInfoDTO> GetEventInfoAsync(int workspaceId, int eventId, string userId)
        {
            var eventGet = await _eventRepository.GetByKeyAsync(eventId);
            eventGet.EventNullChecking();

            var eventCreator = await _userRepository.GetByKeyAsync(eventGet.CreatorId);

            var returnDTO = new EventGetInfoDTO();

            var userEventSpecification = new UserEvents.GetUsersOnEvent(eventId);
            var assignUsers = await _userEventRepository.GetListBySpecAsync(userEventSpecification);

            returnDTO.Users = (List<UserCalendarInfoDTO>)assignUsers;
            returnDTO.CreatorUserName = eventCreator.UserName;
            _mapper.Map(eventGet, returnDTO);

            return returnDTO;
        }
    }
}
