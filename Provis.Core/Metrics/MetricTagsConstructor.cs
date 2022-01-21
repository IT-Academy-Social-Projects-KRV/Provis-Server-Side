

using App.Metrics;

namespace Provis.Core.Metrics
{
    public static class MetricTagsConstructor
    {
        public static MetricTags TaskCountByStatus(int workspaceId, int taskStatusId)
        {
            MetricTags tags = new (
                new[] { "workspace_id", "task_status" },
                new[] { workspaceId.ToString(), taskStatusId.ToString() });
            return tags;
        }
        public static MetricTags MembersCountByWorkspaceRole(int workspaceId, int MemberRoleId)
        {
            MetricTags tags = new(
                new[] { "workspace_id", "member_role" },
                new[] { workspaceId.ToString(), MemberRoleId.ToString() });
            return tags;
        }
    }
}
