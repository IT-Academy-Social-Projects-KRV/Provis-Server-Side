using Ardalis.Specification;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Statuses;
using System;
using System.Linq;

namespace Provis.Core.Entities.UserTaskEntity
{
    public class UserTasks
    {
        internal class UserTaskList : Specification<UserTask, Tuple<int, UserTask, int, int, string>>
        {
            public UserTaskList(string userId, int workspaceId)
            {
                Query
                    .Select(x => new Tuple<int, UserTask, int, int, string>(
                        x.Task.StatusId,
                        x,
                        x.Task.Comments.Count,
                        x.Task.UserTasks.Count,
                        x.Task.TaskCreator.UserName))
                    .Include(x => x.Task)
                    .Where(x => x.UserId == userId && x.Task.WorkspaceId == workspaceId)
                    .OrderBy(x => x.Task.StatusId);
            }
        }

        internal class TaskByUser : Specification<UserTask, EventDTO>
        {
            public TaskByUser(string userId, int workspaceId)
            {
                Query
                    .Select(x => new EventDTO()
                    {
                        EventDay = x.Task.DateOfEnd,
                        Status = CalendarStatuses.TaskDeadline
                    })
                    .Include(x => x.Task)
                    .Where(x => x.Task.DateOfEnd.Month == DateTime.UtcNow.Month && 
                    x.Task.WorkspaceId == workspaceId && 
                    x.UserId == userId && 
                    x.Task.TaskCreatorId != userId &&
                    x.IsUserDeleted == false);
            }
        }

        internal class TaskDayByUser : Specification<UserTask, EventDayDTO>
        {
            public TaskDayByUser(string userId, int workspaceId, DateTime dateTime)
            {
                Query
                    .Select(x => new EventDayDTO()
                    {
                        Status = CalendarStatuses.TaskDeadline,
                        Name = x.Task.Name,
                        DateOfStart = x.Task.DateOfEnd,
                        DateOfEnd = null,
                        AssignedUsers = x.Task.UserTasks.Select(y => new UserCalendarInfoDTO()
                        {
                            UserId = y.UserId,
                            UserName = y.User.UserName
                        }).ToList()
                    })
                    .Include(x => x.Task)
                    .Where(x => x.Task.DateOfEnd.Date == dateTime.Date &&
                    x.Task.WorkspaceId == workspaceId &&
                    x.UserId == userId &&
                    x.Task.TaskCreatorId != userId &&
                    x.IsUserDeleted == false);
            }
        }
        internal class TaskUserList : Specification<UserTask>
        {
            public TaskUserList(int taskId)
            {
                Query
                    .Where(u => u.TaskId == taskId)
                    .Include(u => u.User)
                    .OrderBy(o => o.UserRoleTagId);
            }
        }
        internal class TaskAssignedUserEmailList : Specification<UserTask, string>
        {
            public TaskAssignedUserEmailList(int taskId)
            {
                Query
                    .Select(x => x.User.Email)
                    .Where(t => t.TaskId == taskId);
            }
        }
        internal class AssignedMember : Specification<UserTask>
        {
            public AssignedMember(int TaskId, string userId)
            {
                Query
                    .Where(x => x.TaskId == TaskId && x.UserId == userId);
            }
        }
    }
}
