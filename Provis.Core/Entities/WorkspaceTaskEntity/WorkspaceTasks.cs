using Ardalis.Specification;
using Provis.Core.DTO.EventDTO;
using Provis.Core.Statuses;
using System;
using System.Linq;

namespace Provis.Core.Entities.WorkspaceTaskEntity
{
    public class WorkspaceTasks
    {
        internal class UnassignedTaskList : Specification<WorkspaceTask, Tuple<int, WorkspaceTask>>
        {
            public UnassignedTaskList(int workspaceId)
            {
                Query
                    .Select(x => new Tuple<int, WorkspaceTask>(
                        x.StatusId,
                        x))
                    .Where(x => x.WorkspaceId == workspaceId && !x.UserTasks.Any())
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
                        EventDay = x.DateOfEnd,
                        Status = CalendarStatuses.TaskDeadline
                    })
                    .Include(x => x.UserTasks)
                    .Where(p => p.WorkspaceId == workspaceId &&
                        p.DateOfEnd.Month == DateTime.UtcNow.Month &&
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
                        EventDay = x.DateOfEnd,
                        Status = CalendarStatuses.TaskDeadline
                    })
                    .Where(p => p.WorkspaceId == workspaceId &&
                        p.DateOfEnd.Month == DateTime.UtcNow.Month);
            }
        }
    }
}

//Query
//                    .Select(x => new EventDTO()
//                    {
//                        EventDay = x.DateOfEnd,
//                        Status = CalendarStatuses.Task
//                    })
//                    .Include(x => x.UserTasks)
//                    .ThenInclude(x => x.Task)
//                    .Where(p => p.WorkspaceId == workspaceId &&
//                        p.DateOfEnd.Month == DateTime.UtcNow.Month &&
//                        (p.UserTasks.Exists(x => x.UserId == userId && x.Task.WorkspaceId == workspaceId) ||
//                        p.TaskCreatorId == userId))
