using Provis.Core.Statuses;
using System;

namespace Provis.Core.DTO.EventDTO
{
    public class EventAllDTO
    {
        public DateTime DateTime { get; set; }
        public CalendarStatuses statuses { get; set; }
    }
}
