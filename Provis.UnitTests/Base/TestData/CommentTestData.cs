using Provis.Core.DTO.CommentDTO;
using Provis.Core.DTO.CommentsDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using System;
using System.Collections.Generic;

namespace Provis.UnitTests.Base.TestData
{
    public class CommentTestData
    {
        public static UserWorkspace GetUserWorkspace()
        {
            return new UserWorkspace()
            {
                UserId = "1",
                RoleId = 2,
                WorkspaceId = 1
            };
        }

        public static Comment GetComment()
        {
            return new Comment()
            {
                Id = 1,
                CommentText = "Text1",
                TaskId = 1,
                UserId = "1",
                DateOfCreate = new DateTime(2000, 1, 1, 1, 1, 1),
                User = new User() { UserName = "Username1" }
            };
        }

        public static List<Comment> GetCommentsList()
        {
            return new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    CommentText = "Text1",
                    TaskId = 1,
                    UserId = "1",
                    DateOfCreate = new DateTime(2000, 1, 1, 1, 1, 1),
                    User = new User(){UserName = "Username1"}
                },
                new Comment()
                {
                    Id = 2,
                    CommentText = "Text2",
                    TaskId = 1,
                    UserId = "2",
                    DateOfCreate = new DateTime(2000, 1, 1, 1, 1, 1),
                    User = new User(){UserName = "Username2"}
                },
                new Comment()
                {
                    Id = 3,
                    CommentText = "Text3",
                    TaskId = 1,
                    UserId = "3",
                    DateOfCreate = new DateTime(2000, 1, 1, 1, 1, 1),
                    User = new User(){UserName = "Username3"}
                }
            };
        }

        public static List<CommentListDTO> GetCommentsListDTOs()
        {
            return new List<CommentListDTO>()
            {
                new CommentListDTO()
                {
                    Id = 1,
                    CommentText = "Text1",
                    TaskId = 1,
                    UserId = "1",
                    UserName = "Username1",
                    DateTime = new DateTime(2000, 1, 1, 1, 1, 1)
                },
                new CommentListDTO()
                {
                    Id = 2,
                    CommentText = "Text2",
                    TaskId = 1,
                    UserId = "2",
                    UserName = "Username2",
                    DateTime = new DateTime(2000, 1, 1, 1, 1, 1)
                },
                new CommentListDTO()
                {
                    Id = 3,
                    CommentText = "Text3",
                    TaskId = 1,
                    UserId = "3",
                    UserName = "Username3",
                    DateTime = new DateTime(2000, 1, 1, 1, 1, 1)
                }
            };
        }

        public static EditCommentDTO GetEditCommentDTO()
        {
            return new EditCommentDTO()
            {
                CommentId = 1,
                CommentText = "Edited text",
                WorkspaceId = 1
            };
        }
    }
}