namespace MiniProjectManager.API.Entities
{
    public class User
    {
        public int Id {get; set;}
        public string Username {get; set;} = string.Empty;
        public string Password {get; set;} = string.Empty;

        public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = new List<WorkspaceMember>();
    }
}