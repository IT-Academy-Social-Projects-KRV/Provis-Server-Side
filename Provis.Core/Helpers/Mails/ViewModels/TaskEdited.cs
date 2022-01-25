using Provis.Core.DTO.TaskDTO;
using System;

namespace Provis.Core.Helpers.Mails.ViewModels
{
    public class TaskEdited
    {
        public Uri Uri { get; set; }

        public string WhoEditUserName { get; set; }

        public string WorkspaceName { get; set; }

        public TaskChangeInfoDTO TaskChangeInfo { get; set; }
    }
}
