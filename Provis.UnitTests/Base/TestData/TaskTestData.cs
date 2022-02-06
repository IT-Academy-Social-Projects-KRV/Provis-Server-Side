using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Helpers.Mails;
using System;
using System.Collections.Generic;

namespace Provis.UnitTests.Base.TestData
{
    public class TaskTestData
    {
        public static List<Status> GetTaskStatusesList()
        {
            return new List<Status>()
            {
                new Status()
                {
                   Id = 1,
                   Name = "In review"
                }
            };
        }
        public static List<UserRoleTag> GetWorkerRolesList()
        {
            return new List<UserRoleTag>()
            {
                new UserRoleTag()
                {
                    Id=1,
                    Name = "Developer"
                }
            };
        }
        public static User GetUser()
        {
            return new User()
            {
                Id = "2",
                Email = "test1@gmail.com",
                Name = "Name1",
                Surname = "Surname1",
                UserName = "Username1",
                ImageAvatarUrl = "Path1"
            };
        }
        public static Workspace GetWorkspace()
        {
            return new Workspace()
            {
                Id = 1,
                DateOfCreate = DateTime.Now,
                Description = "Description mock",
                Name = "Provis"
            };
        }
        public static List<string> GetAssignedEmails()
        {
            return new List<string>()
            {
                "test@gmail.com",
                "test1@gmail.com",
                "test2@gmail.com",
                "test3@gmail.com",
                "test4@gmail.com",
            };
        }
        public static WorkspaceTask GetWorkspaceTask()
        {
            return new WorkspaceTask()
            {
                Id = 2,
                Name = "Create workspace",
                DateOfCreate = DateTimeOffset.UtcNow,
                DateOfEnd = DateTimeOffset.UtcNow,
                Description = "Nope",
                StatusId = 1,
                WorkspaceId = 1,
                TaskCreatorId = "2"
            };
        }
        public static TaskAssignDTO GetTaskAssignDTO()
        {
            return new TaskAssignDTO()
            {
                Id = 1,
                WorkspaceId = 2,
                AssignedUser = new UserAssignedOnTaskDTO()
                {
                    UserId = "2",
                    RoleTagId = 2
                }
            };
        }
        public static TaskChangeStatusDTO GetChangeStatusDTO()
        {
            return new TaskChangeStatusDTO()
            {
                WorkspaceId = 1,
                TaskId = 1,
                StatusId = 2
            };
        }
        public static TaskChangeInfoDTO GetTaskChangeInfo()
        {
            return new TaskChangeInfoDTO()
            {
                Id = 1,
                Deadline = DateTimeOffset.UtcNow,
                Description = "New description",
                Name = "New task",
                StoryPoints = 3,
                WorkspaceId = 1
            };
        }
        public static TaskCreateDTO GetTaskCreateDTO()
        {
            return new TaskCreateDTO()
            {
                Name = "Provis",
                DateOfEnd = DateTimeOffset.UtcNow,
                Description = "Create description",
                StatusId = 1,
                WorkspaceId = 2,
                AssignedUsers = new List<UserAssignedOnTaskDTO>()
                {
                    new UserAssignedOnTaskDTO()
                    {
                        UserId = "1",
                        RoleTagId = 1
                    }
                }
            };
        }
        public static UserWorkspace GetUserWorkspace()
        {
            return new UserWorkspace()
            {
                UserId = "1",
                RoleId = 1,
                WorkspaceId = 2
            };
        }
        public static ClientUrl GetClientUrl()
        {
            return new ClientUrl()
            {
                ApplicationUrl = new Uri("http://localhost:4200/")
            };
        }
        public static StatusHistory GetStatusHistory()
        {
            return new StatusHistory()
            {
                Id = 1,
                DateOfChange = DateTimeOffset.UtcNow,
                TaskId = 1,
                StatusId = 1,
                UserId = "1"
            };
        }
        public static List<StatusHistory> GetStatusHistoriesList()
        {
            return new List<StatusHistory>()
            {
                new StatusHistory()
                {
                    Id = 1,
                    DateOfChange = DateTimeOffset.UtcNow,
                    TaskId = 1,
                    StatusId = 1,
                    UserId = "1"
                },
                new StatusHistory()
                {
                    Id = 2,
                    DateOfChange = DateTimeOffset.UtcNow,
                    TaskId = 1,
                    StatusId = 1,
                    UserId = "3"
                },
                new StatusHistory()
                {
                    Id = 3,
                    DateOfChange = DateTimeOffset.UtcNow,
                    TaskId = 1,
                    StatusId = 3,
                    UserId = "4"
                }
            };
        }
        public static List<TaskStatusHistoryDTO> GetTaskStatusHistoryDTOs()
        {
            return new List<TaskStatusHistoryDTO>()
            {
                new TaskStatusHistoryDTO()
                {
                    DateOfChange = new DateTimeOffset(DateTime.UtcNow,TimeSpan.Zero),
                    Status = "Completed",
                    StatusId = 1,
                    UserId = "1",
                    UserName = "Artem"
                },
                new TaskStatusHistoryDTO()
                {
                    DateOfChange = new DateTimeOffset(DateTime.UtcNow,TimeSpan.Zero),
                    Status = "Completed",
                    StatusId = 1,
                    UserId = "3",
                    UserName = "Nazar"
                },
                new TaskStatusHistoryDTO()
                {
                    DateOfChange = new DateTimeOffset(DateTime.UtcNow,TimeSpan.Zero),
                    Status = "In review",
                    StatusId = 3,
                    UserId = "4",
                    UserName = "Vasyl"
                }
            };
        }
        public static List<Tuple<int, UserTask, int, int, string>> GetTasks()
        {
            return new List<Tuple<int, UserTask, int, int, string>>()
            {
                new Tuple<int, UserTask, int, int, string>(
                    1,
                    new UserTask()
                    {
                        IsUserDeleted = false,
                        TaskId = 1,
                        UserId = "1",
                        UserRoleTagId = 1
                    },
                    3,4,"Test")
            };
        }
        public static List<Tuple<int, WorkspaceTask, int, int, string>> GetWorkspaceTasks()
        {
            return new List<Tuple<int, WorkspaceTask, int, int, string>>()
            {
                new Tuple<int, WorkspaceTask, int, int, string>(
                    1,
                    new WorkspaceTask()
                    {
                        Id = 1,
                        Name = "TestTask",
                        DateOfCreate = DateTimeOffset.UtcNow,
                        DateOfEnd = DateTimeOffset.UtcNow,
                        Description = "Test description",
                        StatusId = 1,
                        StoryPoints = 3,
                        TaskCreatorId = "1",
                        WorkspaceId = 2
                    },
                    3,4,"Test1")
            };
        }
        public static Dictionary<int, List<TaskDTO>> GetTasksDTOs()
        {
            return new Dictionary<int, List<TaskDTO>>()
            {
                { 1, new List<TaskDTO>()
                {
                    new TaskDTO()
                    {
                        Id = 1,
                        CommentCount = 1,
                        CreatorUsername ="Test",
                        Deadline = DateTimeOffset.UtcNow,
                        MemberCount = 3,
                        Name = "TestTask",
                        StoryPoints = 3,
                        WorkerRoleId = 2
                    }
                }
                }
            };
        }
    }
}
