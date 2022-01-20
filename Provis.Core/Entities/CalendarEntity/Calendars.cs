using Ardalis.Specification;
using System;
using System.Linq;

namespace Provis.Core.Entities.EventEntity
{
    public class Calendars
    {
        internal class GetAllEvents : Specification<Calendar>
        {
            public GetAllEvents(string userId, int workspaceId)
            {
                Query
                    .Where(c => c.WorkspaceId == workspaceId
                    && c.DateTime.Month == System.DateTime.Today.Month
                    && c.CreatorId == userId
                    || c.IsGeneral);
            }
        }

        internal class GetAllEvents2 : Specification<Calendar, Tuple<>>
        {
            public GetAllEvents2(string userId, int workspaceId)
            {
                
            }
        }
    }
}
