namespace MiniProjectManager.API.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Priority { get; set; } = "Low";

        //Board
        public int BoardId {get; set;}
        public Board? Board{get; set;}

        //Workspace
        public int? AssigneeId {get; set;}
        public User? Assignee{get; set;}

    }
}