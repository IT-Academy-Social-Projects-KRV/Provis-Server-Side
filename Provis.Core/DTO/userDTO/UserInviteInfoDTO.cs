using Provis.Core.Entities;
using System;
namespace Provis.Core.DTO.UserDTO
{
    public class UserInviteInfoDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool? IsConfirm { get; set; }
        public string WorkspaceName { get; set; }
        public string FromUserName { get; set; }
        public string ToUserId { get; set; }
    }
}
