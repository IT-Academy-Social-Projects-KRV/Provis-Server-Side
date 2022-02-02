namespace Provis.Core.DTO.TaskDTO
{
    public class TaskAssignedUsersDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int RoleTagId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
