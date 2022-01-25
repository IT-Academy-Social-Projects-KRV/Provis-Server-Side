using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Histogram;

namespace Provis.Core.Metrics
{
    public static class WorkspaceMetrics
    {
        public static CounterOptions TaskCountByStatus => new CounterOptions
        {
            Context = "TaskCountByStatus",
            Name = "TaskCountByStatus",
            MeasurementUnit = Unit.Calls
        };
        public static CounterOptions MembersCountByWorkspaceRole => new CounterOptions
        {
            Context = "MembersCountByWorkspaceRole",
            Name = "MembersCountByWorkspaceRole",
            MeasurementUnit = Unit.Calls
        };
        public static CounterOptions TaskRolesCountByWorkspace => new CounterOptions
        {
            Context = "TaskRolesCountByWorkspace",
            Name = "TaskRolesCountByWorkspace",
            MeasurementUnit = Unit.Calls
        };
    }
}
