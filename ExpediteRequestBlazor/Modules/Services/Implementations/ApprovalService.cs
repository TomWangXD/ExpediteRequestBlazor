namespace ExpediteRequestBlazor.Modules.Services.Implementations
{
    public class ApprovalService : IApprovalService
    {

        private readonly IApprovalRepository _approvalRepository;
        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;

        public ApprovalService(IDbContextFactory<ExpediteRequestContext> contextFactory, IApprovalRepository approvalRepository)
        {
            _contextFactory = contextFactory;
            _approvalRepository = approvalRepository;
        }

        public async Task Update(Approval approval)
        {
            ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            await _approvalRepository.Update(context, approval);
        }

        public async Task Create(Approval approval)
        {
            ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            await _approvalRepository.Create(context, approval);
        }
    }
}
