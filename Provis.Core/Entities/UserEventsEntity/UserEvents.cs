using Ardalis.Specification;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Statuses;
using System;
using System.Linq;

namespace Provis.Core.Entities.UserEventsEntity
{
    public class UserEvents
    {
        internal class GetMyEvents : Specification<UserEvent, EventDTO>
        {
            public GetMyEvents(string userId, int workspaceId)
            {
                Query
                    .Select(x => new EventDTO()
                    {
                        EventDay = x.Event.DateOfStart,
                        Status = CalendarStatuses.PersonalEventStart
                    })
                    .Where(c => c.Event.WorkspaceId == workspaceId &&
                        c.Event.DateOfStart.Month == DateTime.UtcNow.Month &&
                        c.Event.CreatorId != userId &&
                        c.UserId == userId);
            }
        }

        internal class GetDayEvents : Specification<UserEvent, EventDayDTO>
        {
            public GetDayEvents(string userId, int workspaceId, DateTimeOffset dateTime)
            {
                Query
                    .Select(x => new EventDayDTO()
                    {
                        Status = CalendarStatuses.PersonalEventStart,
                        Name = x.Event.EventName,
                        DateOfStart = x.Event.DateOfStart,
                        DateOfEnd = x.Event.DateOfEnd,
                        AssignedUsers = x.Event.UserEvents.Select(y => new UserCalendarInfoDTO()
                        {
                            UserId = y.UserId,
                            UserName = y.User.UserName
                        }).ToList()
                    })
                    .Where(c => c.Event.WorkspaceId == workspaceId &&
                        c.Event.DateOfStart.Date == dateTime.Date &&
                        c.Event.CreatorId != userId &&
                        c.UserId == userId);
            }
        }

        internal class GetUsersOnEvent : Specification<UserEvent, UserCalendarInfoDTO>
        {
            public GetUsersOnEvent(int eventId)
            {
                Query
                    .Select(x => new UserCalendarInfoDTO()
                    {
                        UserId = x.UserId,
                        UserName = x.User.UserName
                    })
                    .Where(c => c.EventId == eventId);
            }
        }
    }
}
