namespace ExpediteRequestBlazor.Modules.Repositories.Interfaces
{
    public interface IApprovalRepository
    {
        Task Update(ExpediteRequestContext context, Approval approval);
        Task Create(ExpediteRequestContext context, Approval approval);
    }
}
