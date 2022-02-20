using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.CalendarDTO;
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
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId})]
        public async Task<IActionResult> CreateEventAsync([FromBody] EventCreateDTO eventCreateDTO)
        {
            await _calendarService.CreateEventAsync(eventCreateDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId})]
        public async Task<IActionResult> EditEventAsync([FromBody] EventEditDTO eventCreateDTO)
        {
            await _calendarService.EditEventAsync(eventCreateDTO, UserId);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId})]
        [Route("workspace/{workspaceId}/event")]
        public async Task<IActionResult> DeleteEventAsync(int eventId)
        {
            await _calendarService.DeleteEventAsync(eventId, UserId);

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId})]
        [Route("workspace/{workspaceId}/user-event")]
        public async Task<IActionResult> LeaveEventAsync(int eventId, int workspaceId)
        {
            await _calendarService.LeaveEventAsync(eventId, UserId, workspaceId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId, WorkSpaceRoles.ViewerId})]
        [Route("workspace/{workspaceId}/months-events")]
        public async Task<IActionResult> GetAllEventsAsync(int workspaceId)
        {
            var getEvents = await _calendarService.GetAllEventsAsync(workspaceId, UserId);

            return Ok(getEvents);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId, WorkSpaceRoles.ViewerId})]
        [Route("workspace/{workspaceId}/date/{dateTime}")]
        public async Task<IActionResult> GetDayEventsAsync(int workspaceId, DateTimeOffset dateTime)
        {
            var getEvents = await _calendarService.GetDayEventsAsync(workspaceId, dateTime, UserId);

            return Ok(getEvents);
        }

        [Authorize]
        [HttpGet]
        [WorkspaceRoles(new WorkSpaceRoles[] {
            WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId,
            WorkSpaceRoles.MemberId, WorkSpaceRoles.ViewerId})]
        [Route("workspace/{workspaceId}/event")]
        public async Task<IActionResult> GetEventInfoAsync(int workspaceId, int eventId)
        {
            var getEvent = await _calendarService.GetEventInfoAsync(workspaceId, eventId, UserId);

            return Ok(getEvent);
        }
    }
}
