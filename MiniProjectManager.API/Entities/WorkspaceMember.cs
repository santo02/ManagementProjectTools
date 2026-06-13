namespace MiniProjectManager.API.Entities
{
    public class WorkspaceMember
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public Workspace? Workspace { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; } 

        public string Role { get; set; } = "Member"; 
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}