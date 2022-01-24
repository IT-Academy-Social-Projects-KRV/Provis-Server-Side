﻿using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities.UserEntity;
using System;
using System.Collections.Generic;

namespace Provis.Core.DTO.CalendarDTO
{
    public class EventCreateDTO
    {
        public int WorkspaceId { get; set; }

        public string EventName { get; set; }
        public string EventMessage { get; set; }

        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }

        public List<UserAssignOnEventDTO> AssignedUsers { get; set; }
    }
}
