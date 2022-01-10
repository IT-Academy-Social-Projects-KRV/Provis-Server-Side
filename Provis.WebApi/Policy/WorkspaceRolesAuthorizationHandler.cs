using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Linq;
using Provis.Core.Entities;
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
        private readonly IRepository<UserWorkspace> userWorkspaceRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WorkspaceRolesAuthorizationHandler(IRepository<UserWorkspace> userWorkspaceRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.userWorkspaceRepository = userWorkspaceRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkspaceRolesRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Task.CompletedTask;
            }

            int? workspaceId = null;

            if (httpContextAccessor.HttpContext.Request.Method == "POST" || httpContextAccessor.HttpContext.Request.Method == "PUT")
            {
                var syncIoFeature = httpContextAccessor.HttpContext.Features.Get<IHttpBodyControlFeature>();
                syncIoFeature.AllowSynchronousIO = true;

                httpContextAccessor.HttpContext.Request.EnableBuffering();

                using (var reader = new StreamReader(httpContextAccessor.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    var bodyStr = "";
                    bodyStr = reader.ReadToEnd();

                    JObject jObj = JObject.Parse(bodyStr);
                    workspaceId = (int?)jObj["workspaceId"];

                    httpContextAccessor.HttpContext.Request.Body.Position = 0;
                }
            }

            if ((httpContextAccessor.HttpContext.Request.Method == "GET" || httpContextAccessor.HttpContext.Request.Method == "DELETE")
                && httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue("workspaceId", out object obj)
                && int.TryParse(obj.ToString(), out int id))
            {
                workspaceId = id;
            }

            if (!workspaceId.HasValue)
            {
                return Task.CompletedTask;
            }

            var userWorkspace = userWorkspaceRepository
                .Query()
                .FirstOrDefault(x =>
                    x.UserId == userId
                    && x.WorkspaceId == workspaceId
                    && requirement.WorkspaceRolesId.Contains(x.RoleId));

            if (userWorkspace != null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
