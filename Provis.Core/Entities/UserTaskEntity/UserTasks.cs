﻿using Ardalis.Specification;
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
      
        internal class AssignedMember : Specification<UserTask>
        {
            public AssignedMember(int TaskId,string userId)
            {
                Query
                    .Where(x => x.TaskId == TaskId && x.UserId == userId);
            }
        }
    }
}
