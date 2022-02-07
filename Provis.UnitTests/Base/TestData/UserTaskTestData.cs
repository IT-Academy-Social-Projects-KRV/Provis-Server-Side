using Provis.Core.Entities.UserTaskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.UnitTests.Base.TestData
{
    public class UserTaskTestData
    {
        public static List<UserTask> GetTestUserTaskList()
        {
            return new List<UserTask>()
            {
                new UserTask()
                {
                    UserId = "1",
                    TaskId = 1,
                    UserRoleTagId = 1,
                    IsUserDeleted = false
                },

                new UserTask()
                {
                    UserId = "1",
                    TaskId = 2,
                    UserRoleTagId = 2,
                    IsUserDeleted = true
                }
            };
        }

        public static List<Tuple<int, UserTask, int, int, string>> GetWorkspaceUserTasks()
        {
            return new List<Tuple<int, UserTask, int, int, string>>()
            {
                new Tuple<int, UserTask, int, int, string>(
                    1,
                    new UserTask()
                    {
                        TaskId = 1,
                        UserId = "1",
                        UserRoleTagId = 1,
                        IsUserDeleted = false
                    },
                    3,4,"Test1")
            };
        }
    }
}
