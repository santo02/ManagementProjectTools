namespace MiniProjectManager.API.DTOs
{
    public class WorkspaceResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int LeaderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorName { get; set; } = string.Empty;

    }

    public class CreateWorkspaceDto
    {
        public string Name { get; set; } = string.Empty;
    }
}