using Ardalis.Specification;
using Provis.Core.Entities.WorkspaceTaskEntity;
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
    }
}
