using System;

namespace Provis.Core.DTO.WorkspaceDTO
{
    public class WorkspaceInviteInfoDTO
    {
        public DateTimeOffset Date { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public string ToUserEmail { get; set; }
        public int InviteId { get; set; }
    }
}
