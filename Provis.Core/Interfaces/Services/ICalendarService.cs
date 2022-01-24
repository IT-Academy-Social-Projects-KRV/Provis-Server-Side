﻿using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ICalendarService
    {
        Task<List<EventDTO>> GetAllEventsAsync(int workspaceId, string userId);
        Task CreateEventAsync(EventCreateDTO eventCreateDTO, string userId);
    }
}
