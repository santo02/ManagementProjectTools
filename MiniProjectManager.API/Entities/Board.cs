
namespace MiniProjectManager.API.Entities {
    public class Board{ 
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Position { get; set; }
        
        public int WorkspaceId { get; set; }
        public Workspace? Workspace { get; set; }
    }
}