using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        protected readonly ICalendarService _calendarService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;
        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId,
            WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId,
            WorkSpaceRoles.ViewerId})]
        [Route("workspace/{workspaceId}/allevents")]
        public async Task<IActionResult> GetAllEvents(int workspaceId)
        {
            var getEvents = await _calendarService.GetAllEvents(workspaceId, UserId);

            return Ok(getEvents);
        }
    }
}
