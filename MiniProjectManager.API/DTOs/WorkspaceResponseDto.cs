namespace MiniProjectManager.API.DTOs
{
        public class WorkspaceResponseDto
    {
        public int Id{get; set;}
        public string Name{get; set;} = string.Empty;
        public DateTime CreatedAt {get; set;}
        public string CreatorName { get; set; } = string.Empty;

    }
}