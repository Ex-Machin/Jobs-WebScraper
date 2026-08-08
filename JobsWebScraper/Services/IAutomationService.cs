namespace TaskManager.Services
{
    public interface IAutomationService
    {
        Task RunAutomationISS();
        Task RunAutomationMacgregor();
        Task RunAutomationAlior();
        Task RunAutomationTfbank();
    }
}
