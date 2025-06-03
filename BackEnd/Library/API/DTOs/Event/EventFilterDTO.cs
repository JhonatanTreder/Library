namespace API.DTOs.Event
{
    public class EventFilterDTO
    {
        public string? Title { get; set; }
        public string? TargetAudience { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
