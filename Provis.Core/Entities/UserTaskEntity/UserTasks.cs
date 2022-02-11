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
        internal class UserTaskList : Specification<UserTask, Tuple<int, UserTask>>
        {
            public UserTaskList(string userId, int workspaceId)
            {
                Query
                    .Select(x => new Tuple<int, UserTask>(
                        x.Task.StatusId,
                        x))
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
    }
}
