using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Provis.WebApi.Policy
{
    public class WorkspaceRolesPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "WorkspaceRoles";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public WorkspaceRolesPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var workspaceRoles = policyName.Substring(POLICY_PREFIX.Length + 1).Split(":");

            if (workspaceRoles != null)
            {
                var workspaceRolesId = Array.ConvertAll(workspaceRoles, int.Parse);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new WorkspaceRolesRequirement(workspaceRolesId));
                return Task.FromResult(policy.Build());
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
