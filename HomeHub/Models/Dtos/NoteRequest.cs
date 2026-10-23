namespace HomeHub.Models.Dtos
{
    public class NoteRequest
    {
        public string Content { get; set; } = string.Empty;
        public string Kind { get; set; } = "note";
        public int? RoomId { get; set; }
        public int? CategoryId { get; set; }
    }
}
