namespace TaskManager.Services
{
    public interface IAutomationService
    {
        Task<int> RunAutomationISS();
        Task<int> RunAutomationMacgregor();
    }
}
