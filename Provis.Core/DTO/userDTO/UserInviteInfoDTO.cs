using Provis.Core.Entities;
using System;
namespace Provis.Core.DTO.userDTO
{
    public class UserInviteInfoDTO
    {
        public DateTime Date { get; set; }
        public bool? IsConfirm { get; set; }
        public int WorkspaceId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
    }
}
