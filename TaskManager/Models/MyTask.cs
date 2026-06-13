


namespace TaskManager.Models
{
    public enum Status
    {
        Created,
        InProgress,
        Completed
    }
    public class MyTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Status Status { get; set; }
    }
}
