namespace MiniProjectManager.API.DTOs
{
    public class CreateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BoardId { get; set; }
        public int? AssigneeId { get; set; }

        public string Priority { get; set; } = string.Empty;
    }

    public class TaskItemResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Priority { get; set; } = string.Empty;

        public int BoardId { get; set; }
        public string BoardTitle { get; set; } = string.Empty;

        public int? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }

    }

    public class UpdateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;

        public int BoardId { get; set; }
        public int? AssigneeId { get; set; }
    }
}