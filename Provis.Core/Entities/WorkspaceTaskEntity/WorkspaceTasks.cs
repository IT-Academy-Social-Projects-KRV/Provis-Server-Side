using Ardalis.Specification;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Statuses;
using System;
using System.Linq;

namespace Provis.Core.Entities.WorkspaceTaskEntity
{
    public class WorkspaceTasks
    {
        internal class UnassignedTaskList : Specification<WorkspaceTask, Tuple<int, WorkspaceTask, int, int, string>>
        {
            public UnassignedTaskList(int workspaceId)
            {
                Query
                    .Select(x => new Tuple<int, WorkspaceTask, int, int, string>(
                        x.StatusId,
                        x,
                        x.Comments.Count,
                        x.UserTasks.Count,
                        x.TaskCreator.UserName))
                    .Where(x => x.WorkspaceId == workspaceId && (!x.UserTasks.Any() || x.UserTasks.All(y => y.IsUserDeleted == true)))
                    .OrderBy(x => x.StatusId);
            }
        }

        internal class TaskById : Specification<WorkspaceTask>
        {
            public TaskById(int taskId)
            {
                Query
                    .Where(p => p.Id == taskId)
                    .Include(p => p.UserTasks
                        .Where(x => !x.IsUserDeleted))
                        .ThenInclude(x => x.User);
            }
        }

        internal class TaskByUser : Specification<WorkspaceTask, EventDTO>
        {
            public TaskByUser(string userId, int workspaceId)
            {
                Query
                    .Select(x => new EventDTO()
                    {
                        EventDay = x.DateOfEnd.UtcDateTime,
                        Status = CalendarStatuses.TaskDeadline
                    })
                    .Include(x => x.UserTasks)
                    .Where(p => p.WorkspaceId == workspaceId &&
                        p.DateOfEnd.Month == DateTime.UtcNow.Month &&
                        p.TaskCreatorId == userId);
            }
        }

        internal class TaskDayByUser : Specification<WorkspaceTask, EventDayDTO>
        {
            public TaskDayByUser(string userId, int workspaceId, DateTimeOffset dateTime)
            {
                Query
                    .Select(x => new EventDayDTO()
                    {
                        Status = CalendarStatuses.TaskDeadline,
                        Name = x.Name,
                        DateOfStart = x.DateOfEnd.UtcDateTime,
                        DateOfEnd = null,
                        AssignedUsers = x.UserTasks.Select(y => new UserCalendarInfoDTO()
                        {
                            UserId = y.UserId,
                            UserName = y.User.UserName
                        }).ToList()
                    })
                    .Include(x => x.UserTasks)
                    .Where(p => p.WorkspaceId == workspaceId &&
                        p.DateOfEnd.Date == dateTime.Date &&
                        p.TaskCreatorId == userId);
            }
        }

        internal class AllWorkspaceTasks : Specification<WorkspaceTask, EventDTO>
        {
            public AllWorkspaceTasks(int workspaceId)
            {
                Query
                    .Select(x => new EventDTO()
                    {
                        EventDay = x.DateOfEnd.UtcDateTime,
                        Status = CalendarStatuses.TaskDeadline
                    })
                    .Where(p => p.WorkspaceId == workspaceId &&
                        p.DateOfEnd.Month == DateTime.UtcNow.Month);
            }
        }

        internal class AllWorkspaceDayTasks : Specification<WorkspaceTask, EventDayDTO>
        {
            public AllWorkspaceDayTasks(int workspaceId, DateTimeOffset dateTime)
            {
                Query
                    .Select(x => new EventDayDTO()
                    {
                        Status = CalendarStatuses.TaskDeadline,
                        Name = x.Name,
                        DateOfStart = x.DateOfEnd.UtcDateTime,
                        DateOfEnd = null,
                        AssignedUsers = x.UserTasks.Select(y => new UserCalendarInfoDTO()
                        {
                            UserId = y.UserId,
                            UserName = y.User.UserName
                        }).ToList()
                    })
                    .Where(p => p.WorkspaceId == workspaceId &&
                        p.DateOfEnd.Date == dateTime.Date);
            }
        }
    }
}
