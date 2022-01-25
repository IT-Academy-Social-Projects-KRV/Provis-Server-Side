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
        public static MetricTags MembersCountByWorkspaceRole(int workspaceId, int memberRoleId)
        {
            MetricTags tags = new(
                new[] { "workspace_id", "member_role" },
                new[] { workspaceId.ToString(), memberRoleId.ToString() });
            return tags;
        }
        public static MetricTags TaskRolesCountByWorkspace(int workspaceId, int taskRoleId)
        {
            MetricTags tags = new(
                new[] { "workspace_id", "task_role" },
                new[] { workspaceId.ToString(), taskRoleId.ToString() });
            return tags;
        }
    }
}
