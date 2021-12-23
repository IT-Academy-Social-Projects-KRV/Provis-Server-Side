using Microsoft.AspNetCore.Authorization;
using Provis.Core.Roles;
using System;

namespace Provis.WebApi.Policy
{
    public class WorkspaceRolesAttribute : AuthorizeAttribute
    {

        const string POLICY_PREFIX = "WorkspaceRoles";

        public WorkspaceRolesAttribute(WorkSpaceRoles[] workspaceRolesId) 
            => WorkspaceRolesId = Array.ConvertAll(workspaceRolesId, value => (int)value);

        public int[] WorkspaceRolesId
        {
            get
            {
                var workspaceRoles = Policy.Substring(POLICY_PREFIX.Length + 1).Split(":");

                var workspaceRolesId = Array.ConvertAll(workspaceRoles, int.Parse);
                return workspaceRolesId;
            }
            set
            {
                Policy = $"{POLICY_PREFIX}:{String.Join(":", value)}";
            }
        }
    }
}
