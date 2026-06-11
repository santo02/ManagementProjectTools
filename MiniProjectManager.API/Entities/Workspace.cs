
namespace MiniProjectManager.API.Entities
{
    public class Workspace { 
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int LeaderId { get; set; }

        public DateTime CreatedAt { get; set; }
        public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
        public ICollection<Board> Boards { get; set; } = new List<Board>();

        public int UserId {get; set;}
        public User? User { get; set; }
    }

}