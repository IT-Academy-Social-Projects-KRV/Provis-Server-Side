using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.EventDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ICalendarService
    {
        Task<List<EventAllDTO>> GetAllEvents(int workspaceId, string userId);
    }
}
