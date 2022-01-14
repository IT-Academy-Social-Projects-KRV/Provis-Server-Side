using Ardalis.Specification;
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
                    .Include(p => p.UserTasks);
            }
        }
    }
}
