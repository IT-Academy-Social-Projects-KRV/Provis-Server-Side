using Provis.Core.DTO.UserDTO;
using Provis.Core.Statuses;
using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.EventDTO
{
    public class EventDTO
    {
        public DateTimeOffset EventDay { get; set; }

        public CalendarStatuses Status { get; set; }
    }
}
