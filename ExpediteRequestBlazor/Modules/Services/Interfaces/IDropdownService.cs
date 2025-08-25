namespace ExpediteRequestBlazor.Modules.Services.Interfaces
{
    public interface IDropdownService
    {
        Task<List<string>> GetExpediteReasons();
        Task<List<string>> GetApprovalStatuses();
    }
}
