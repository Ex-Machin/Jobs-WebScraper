namespace TaskManager.Models
{
    public enum JobStatus
    {
        Applied,
        Rejected,
        Accepted
    }
    public class Job
    {

        public int Id { get; set; }
        public string Company { get; set; } = null!;
        public JobStatus Status { get; set; }
        public int InterviewRound { get; set; }

        public string Title { get; set; } = null!;
        public string Departement { get; set; } = null!; // to fix department*
        public string Region { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}
