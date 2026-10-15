namespace HomeHub.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public string Kind { get; set; } = "note";
        public int? RoomId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
