using Ardalis.Specification;
using System;
using System.Linq.Expressions;

namespace Provis.Core.Entities.UserTaskEntity
{
    public class UserTasks
    {
        internal class UserTaskList : Specification<UserTask, Tuple<int, UserTask, int, int, string>>
        {
            public UserTaskList(string userId, int workspaceId, int? sprintId)
            {
                SetQuery(x => x.UserId == userId &&
                    x.Task.WorkspaceId == workspaceId &&
                    x.Task.SprintId == sprintId);
            }

            public UserTaskList(string userId, int workspaceId)
            {
                SetQuery(x => x.UserId == userId &&
                    x.Task.WorkspaceId == workspaceId);
            }

            private void SetQuery(Expression<Func<UserTask, bool>> whereCriteria)
            {
                Query
                    .Select(x => new Tuple<int, UserTask, int, int, string>(
                        x.Task.StatusId,
                        x,
                        x.Task.Comments.Count,
                        x.Task.UserTasks.Count,
                        x.Task.TaskCreator.UserName))
                    .Include(x => x.Task)
                    .Where(whereCriteria)
                    .OrderBy(x => x.Task.StatusId);
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
