namespace Provis.Core.DTO.WorkspaceDTO
{
    public class WorkspaceInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Role { get; set; }

        public bool isUseSprints { get; set; }
    }
}
