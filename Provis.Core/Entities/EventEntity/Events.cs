using Ardalis.Specification;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Statuses;
using System;
using System.Linq;

namespace Provis.Core.Entities.EventEntity
{
    public class Events
    {
        internal class GetMyEvents : Specification<Event, EventDTO>
        {
            public GetMyEvents(string userId, int workspaceId)
            {
                Query
                    .Select(x => new EventDTO()
                    {
                        EventDay = x.DateOfStart.UtcDateTime,
                        Status = x.IsCreatorExist ? CalendarStatuses.PersonalEventStart : CalendarStatuses.SomebodysEventStart
                    })
                    .Where(c => c.WorkspaceId == workspaceId &&
                        c.DateOfStart.Month == DateTimeOffset.UtcNow.Month &&
                        c.CreatorId == userId);
            }
        }

        internal class GetDayEvents : Specification<Event, EventDayDTO>
        {
            public GetDayEvents(string userId, int workspaceId, DateTimeOffset dateTime)
            {
                Query
                    .Select(x => new EventDayDTO()
                    {
                        Status = x.IsCreatorExist ? CalendarStatuses.PersonalEventStart : CalendarStatuses.SomebodysEventStart,
                        Name = x.EventName,
                        DateOfStart = x.DateOfStart.UtcDateTime,
                        DateOfEnd = x.DateOfEnd,
                        AssignedUsers = x.UserEvents.Select(y => new UserCalendarInfoDTO()
                        {
                            UserId = y.UserId,
                            UserName = y.User.UserName
                        }).ToList()
                    })
                    .Where(c => c.WorkspaceId == workspaceId &&
                        c.DateOfStart.Date == dateTime.Date &&
                        c.CreatorId == userId);
            }
        }
    }
}
