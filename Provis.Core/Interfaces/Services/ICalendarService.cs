using Provis.Core.DTO.CalendarDTO;
using Provis.Core.DTO.EventDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ICalendarService
    {
        Task CreateEventAsync(EventCreateDTO eventCreateDTO, string userId);
        Task EditEventAsync(EventEditDTO eventCreateDTO, string userId);
        Task<List<EventDTO>> GetAllEventsAsync(int workspaceId, string userId);
        Task<List<EventDayDTO>> GetDayEventsAsync(int workspaceId, DateTimeOffset dateTime, string userId);
    }
}
