using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Interfaces.Repositories;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Provis.WebApi.Policy
{
    public class WorkspaceRolesAuthorizationHandler : AuthorizationHandler<WorkspaceRolesRequirement>
    {
        private readonly IEntityRepository<UserWorkspace> userWorkspaceRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WorkspaceRolesAuthorizationHandler(IEntityRepository<UserWorkspace> userWorkspaceRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.userWorkspaceRepository = userWorkspaceRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkspaceRolesRequirement requirement)
        {
            var request = httpContextAccessor.HttpContext.Request;

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Task.CompletedTask;
            }

            int? workspaceId = null;

            if (request.RouteValues.TryGetValue("workspaceId", out object obj) &&
                int.TryParse(obj.ToString(), out int id))
            {
                workspaceId = id;
            }

            if (workspaceId == null &&
                request.ContentType.StartsWith("application/json"))
            {
                var syncIoFeature = httpContextAccessor.HttpContext.Features.Get<IHttpBodyControlFeature>();
                syncIoFeature.AllowSynchronousIO = true;

                request.EnableBuffering();

                using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    var bodyStr = "";
                    bodyStr = reader.ReadToEnd();

                    JObject jObj = JObject.Parse(bodyStr);
                    workspaceId = (int?)jObj["workspaceId"];

                    request.Body.Position = 0;
                }
            }

            if (workspaceId == null &&
                request.ContentType.StartsWith("multipart/form-data") &&
                request.Form.TryGetValue("workspaceId", out StringValues formWorkspaceId) &&
                int.TryParse(formWorkspaceId, out int formWorkspaceIdInt))
            {
                workspaceId = formWorkspaceIdInt;
            }

            if (!workspaceId.HasValue)
            {
                return Task.CompletedTask;
            }

            var userWorkspace = userWorkspaceRepository
                .Query()
                .FirstOrDefault(x =>
                    x.UserId == userId &&
                    x.WorkspaceId == workspaceId &&
                    requirement.WorkspaceRolesId.Contains(x.RoleId));

            if (userWorkspace != null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
