namespace HomeHub.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Recurrence { get; set; } = "none";
    }
}
