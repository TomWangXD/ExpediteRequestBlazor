namespace ExpediteRequestBlazor.Modules.Services.Interfaces
{
    public interface IApprovalService
    {
        Task Update(Approval approval);
        Task Create(Approval approval);
    }
}
