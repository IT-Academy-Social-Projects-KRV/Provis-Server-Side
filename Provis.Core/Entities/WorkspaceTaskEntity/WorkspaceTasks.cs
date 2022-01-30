using Ardalis.Specification;
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
    }
}
