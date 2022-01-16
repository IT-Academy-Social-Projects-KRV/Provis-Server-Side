using System;

namespace Provis.Core.Helpers.Mails.ViewModels
{
    public class WorkspaceInvite
    {
        public string Owner { get; set; }
        public string WorkspaceName { get; set; }
        public string UserName { get; set; }
        public Uri Uri { get; set; }
    }
}
