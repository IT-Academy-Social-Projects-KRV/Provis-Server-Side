using Provis.Core.DTO.UserDTO;
using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.CalendarDTO
{
    public class EventGetInfoDTO
    {
        public string EventName { get; set; }
        public string EventMessage { get; set; }

        public DateTimeOffset DateOfStart { get; set; }
        public DateTimeOffset? DateOfEnd { get; set; }

        public string CreatorId { get; set; }
        public string CreatorUserName { get; set; }

        public List<UserCalendarInfoDTO> Users { get; set; }
    }
}
