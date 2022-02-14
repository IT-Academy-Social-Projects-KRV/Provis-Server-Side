using Provis.Core.Statuses;
using System;

namespace Provis.Core.DTO.EventDTO
{
    public class EventDTO
    {
        public DateTimeOffset EventDay { get; set; }

        public CalendarStatuses Status { get; set; }
    }
}
