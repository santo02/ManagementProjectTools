namespace MiniProjectManager.API.DTOs
{
    public class BoardResponseDto
    {
        public int Id {get; set;}
        public string Title {get; set;} = string.Empty;
        public int Position { get; set; }
        public int WorkspaceId{get; set;}
    }
}