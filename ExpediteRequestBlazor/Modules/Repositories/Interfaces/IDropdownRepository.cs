namespace ExpediteRequestBlazor.Modules.Repositories.Interfaces
{
    public interface IDropdownRepository
    {
        Task<List<string>> GetExpediteReasons(ExpediteRequestContext context);
        Task<List<string>> GetApprovalStatuses(ExpediteRequestContext context);
    }
}
