namespace TaskManager.Models
{
    public class Job
    {

        public int Id { get; set; }
        public string Company { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Link { get; set; } = null!;

        public DateTime? DatePublished { get; set; }
    }
}
