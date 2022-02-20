using System;

namespace Provis.Core.DTO.CalendarDTO
{
    public class EventEditDTO
    {
        public int WorkspaceId { get; set; }
        public int EventId { get; set; }

        public string EventName { get; set; }
        public string EventMessage { get; set; }

        public DateTimeOffset DateOfStart { get; set; }
        public DateTimeOffset? DateOfEnd { get; set; }
    }
}
