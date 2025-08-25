namespace ExpediteRequestBlazor.Modules.Repositories.Implementations
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;
        public readonly ILogger<DocumentRepository> _logger;

        public ApprovalRepository(IDbContextFactory<ExpediteRequestContext> contextFactory, ILogger<DocumentRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }
        public async Task Update(ExpediteRequestContext context, Approval approval)
        {
            context.Approvals.Update(approval);
            await context.SaveChangesAsync();
        }

        public async Task Create(ExpediteRequestContext context, Approval approval)
        {
            await context.Approvals.AddAsync(approval);
            await context.SaveChangesAsync();
        }
    }
}
