using Ardalis.Specification;
using Provis.Core.DTO.EventDTO;
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
                        EventDay = x.DateOfStart,
                        Status = x.IsCreatorExist?CalendarStatuses.PersonalEventStart:CalendarStatuses.SomebodysEventStart
                    })
                    .Where(c => c.WorkspaceId == workspaceId &&
                        c.DateOfStart.Month == DateTime.UtcNow.Month &&
                        c.CreatorId == userId);
            }
        }
    }
}
