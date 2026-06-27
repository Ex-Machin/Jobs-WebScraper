namespace TaskManager.Models
{
    public enum TaskState
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
        public TaskState State { get; set; }
    }
}
