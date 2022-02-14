using Provis.Core.DTO.UserDTO;
using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.CalendarDTO
{
    public class EventDayDTO
    {
        public CalendarStatuses Status { get; set; }

        public string Name { get; set; }

        public DateTimeOffset DateOfStart { get; set; }
        public DateTimeOffset? DateOfEnd { get; set; }

        public List<UserCalendarInfoDTO> AssignedUsers { get; set; }
    }
}
