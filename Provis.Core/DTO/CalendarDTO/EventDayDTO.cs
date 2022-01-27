using Provis.Core.DTO.UserDTO;
using Provis.Core.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.DTO.CalendarDTO
{
    public class EventDayDTO
    {
        public CalendarStatuses Status { get; set; }

        public string Name { get; set; }

        public DateTime DateOfStart { get; set; }
        public DateTime? DateOfEnd { get; set; }

        public List<UserCalendarInfoDTO> AssignedUsers { get; set; }
    }
}
