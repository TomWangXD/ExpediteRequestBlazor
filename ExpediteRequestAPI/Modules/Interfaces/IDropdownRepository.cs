namespace ExpediteRequestAPI.Modules.Interfaces
{
    public interface IDropdownRepository
    {
        Task<List<string>> GetApprovalStatuses();
        Task<List<string>> GetExpediteReasons();

    }
}
