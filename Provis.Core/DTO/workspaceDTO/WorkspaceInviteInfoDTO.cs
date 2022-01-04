using Provis.Core.Entities;
using System;

namespace Provis.Core.DTO.workspaceDTO
{
    public class WorkspaceInviteInfoDTO
    {
        public DateTime Date { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public string ToUserEmail { get; set; }
        public int InviteId { get; set; }
    }
}
