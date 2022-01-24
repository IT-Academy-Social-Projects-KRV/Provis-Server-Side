using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.Entities;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.EventEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserEventsEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.Core.Statuses;
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

        public async Task<List<EventDTO>> GetAllEventsAsync(int workspaceId, string userId)
        {
            //var calendarSpecification = new UserCalendars.UsersInTasks(workspaceId, userId);
            //var calendarList = await _userCalendarRepository.GetListBySpecAsync(calendarSpecification);

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


        public async Task CreateEventAsync(EventCreateDTO eventCreateDTO, string userId)
        {
            var workspaceEvent = new Event()
            {
                CreatorId = userId,
                IsCreatorExist = false
            };
            _mapper.Map(eventCreateDTO, workspaceEvent);

            List<UserEvent> userEvents = new List<UserEvent>();
            foreach (var item in eventCreateDTO.AssignedUsers)
            {
                if (userEvents.Exists(x => x.UserId == item.UserId))
                {
                    throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                        "This user already assigned");
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
            await _eventRepository.AddAsync(workspaceEvent);
            await _eventRepository.SaveChangesAsync();

            await _userEventRepository.AddRangeAsync(userEvents);
            await _userEventRepository.SaveChangesAsync();
        }
    }
}
