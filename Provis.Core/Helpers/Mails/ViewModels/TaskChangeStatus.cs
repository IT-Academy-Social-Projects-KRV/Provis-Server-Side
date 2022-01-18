using Provis.Core.Statuses;
using System;

namespace Provis.Core.Helpers.Mails.ViewModels
{
    public class TaskChangeStatus
    {
        public Uri Uri { get; set; }

        public string WhoChangedUserName { get; set; }

        public TaskStatuses FromStatus { get; set; }

        public TaskStatuses ToStatus { get; set; }

        public string TaskName { get; set; }

        public string UserName { get; set; }

        public string WorkspaceName { get; set; }
    }
}
