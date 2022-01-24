using Ardalis.Specification;
using System.Linq;
using System;
using Provis.Core.DTO.EventDTO;
using Provis.Core.Statuses;

namespace Provis.Core.Entities.UserEntity
{
    public class Users
    {
        internal class UserDateOfBirth : Specification<User, EventDTO>
        {
            public UserDateOfBirth(string userId, int workspaceId)
            {
                Query
                    .Select(x => new EventDTO()
                    {
                        EventDay = x.BirthDate,
                        Status = CalendarStatuses.BirthDay
                    })
                    .Where(x => x.BirthDate.Month == DateTime.UtcNow.Month &&
                    x.UserWorkspaces.Exists(x => x.WorkspaceId == workspaceId && x.UserId == userId));
            }
        }
    }
}
