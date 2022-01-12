using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Provis.Core.Entities.WorkspaceTaskEntity
{
    public class WorkspaceTasks
    {
        internal class GetUserTasks
        {
            private readonly string userId;
            private readonly int workspaceId;
            public IQueryable<WorkspaceTask> Query { get; }
            //public GetUserTasks(string userId, int workspaceId)
            //{
            //    this.userId = userId;
            //    this.workspaceId = workspaceId;

            //    Query
            //        .Include(x => x.Task)
            //        .Where(x => x.UserId == userId && x.Task.WorkspaceId == workspaceId)
            //        .OrderBy(x => x.Task.StatusId)
            //        .Select(x => new Tuple<int, TaskDTO>(
            //            x.Task.StatusId,
            //            _mapper.Map<TaskDTO>(x)))
            //        .ToListAsync();
            //}
        }

    }
}
