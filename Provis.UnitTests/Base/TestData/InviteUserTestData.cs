using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.InviteUserEntity;
using System;
using System.Collections.Generic;

namespace Provis.UnitTests.Base.TestData
{
    public class InviteUserTestData
    {
        public static List<InviteUser> GetInviteUserList()
        {
            return new List<InviteUser>()
            {
                new InviteUser()
                {
                    Id = 1,
                    FromUserId = "2",
                    ToUserId = "1",
                    IsConfirm = true,
                    WorkspaceId = 1
                },
                new InviteUser()
                {
                    Id = 2,
                    FromUserId = "2",
                    ToUserId = "1",
                    IsConfirm = false,
                    WorkspaceId = 2
                },
                new InviteUser()
                {
                    Id = 3,
                    FromUserId = "2",
                    ToUserId = "1",
                    IsConfirm = null,
                    WorkspaceId = 3
                }
            };
        }
    }
}
