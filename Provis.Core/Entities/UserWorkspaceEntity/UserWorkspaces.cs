using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Statuses;
using System;
using System.Linq;

namespace Provis.Core.Entities.UserWorkspaceEntity
{
    public class UserWorkspaces
    {
        internal class WorkspaceList : Specification<UserWorkspace>
        {
            public WorkspaceList(string userId)
            {
                Query
                    .Where(y => y.UserId == userId)
                    .Include(x => x.Workspace)
                    .OrderBy(x => x.RoleId)
                    .ThenBy(x => x.Workspace.Name);
            }
        }

        internal class WorkspaceMember : Specification<UserWorkspace>
        {
            public WorkspaceMember(string userId, int workspaceId)
            {
                Query
                    .Where(p => p.UserId == userId &&
                    p.WorkspaceId == workspaceId)
                    .Include(x => x.User);
            }
        }

        internal class WorkspaceMemberList : Specification<UserWorkspace>
        {
            public WorkspaceMemberList(int workspaceId)
            {
                Query
                    .Where(u => u.WorkspaceId == workspaceId)
                    .Include(u => u.User)
                    .OrderBy(o => o.RoleId);
            }
        }

        internal class WorkspaceInfo : Specification<UserWorkspace>
        {
            public WorkspaceInfo(string userId, int workspaceId)
            {
                Query
                    .Where(x => x.WorkspaceId == workspaceId
                           && x.UserId == userId)
                    .Include(x => x.Workspace);
            }
        }

        internal class WorkspaceOwnerManager : Specification<UserWorkspace>
        {
            public WorkspaceOwnerManager(string userId, int workspaceId)
            {
                Query
                    .Where(p => p.UserId == userId &&
                    p.WorkspaceId == workspaceId &&
                    (p.RoleId == 1 || p.RoleId == 2));
            }
        }

        internal class UsersDateOfBirthDetail : Specification<UserWorkspace, EventDayDTO>
        {
            public UsersDateOfBirthDetail(int workspaceId, DateTimeOffset dateTime)
            {
                Query
                    .Select(x => new EventDayDTO()
                    {
                        Status = CalendarStatuses.BirthDay,
                        Name = "BirthDay",
                        DateOfStart = (DateTimeOffset)x.User.BirthDate,
                        DateOfEnd = null,
                        AssignedUsers = x.User.UserWorkspaces.Select(y => new UserCalendarInfoDTO()
                        {
                            UserId = y.User.Id,
                            UserName = y.User.UserName
                        }).ToList()
                    })
                    .Where(x => x.User.BirthDate.Value.Date == dateTime.Date &&
                    x.WorkspaceId == workspaceId);
            }
        }

        internal class UsersDateOfBirth : Specification<UserWorkspace, EventDTO>
        {
            public UsersDateOfBirth(int workspaceId)
            {
                Query
                    .Select(x => new EventDTO()
                    {
                        EventDay = (DateTimeOffset)x.User.BirthDate,
                        Status = CalendarStatuses.BirthDay
                    })
                    .Where(x => x.User.BirthDate.Value.Month == DateTime.UtcNow.Month &&
                    x.WorkspaceId == workspaceId);
            }
        }
    }
}
