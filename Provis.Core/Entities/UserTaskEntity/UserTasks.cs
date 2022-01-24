using Ardalis.Specification;
using Provis.Core.DTO.EventDTO;
using Provis.Core.Statuses;
using System;

namespace Provis.Core.Entities.UserTaskEntity
{
    public class  UserTasks
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
    }
}