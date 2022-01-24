using Ardalis.Specification;
using Provis.Core.DTO.EventDTO;
using Provis.Core.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
