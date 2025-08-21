using ExpediteRequestAPI.Modules.Interfaces;


namespace ExpediteRequestAPI.Modules.Repositories
{
    public class DropdownRepository : IDropdownRepository
    {
        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;
        public readonly ILogger<DocumentRepository> _logger;
        private readonly IConfiguration _config;
        private readonly Shared.User _user;

        public DropdownRepository(IDbContextFactory<ExpediteRequestContext> contextFactory, ILogger<DocumentRepository> logger, IConfiguration config, Shared.User user)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _config = config;
            _user = user;
        }


        public async Task<List<string>> GetExpediteReasons()
        {
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            List<string> reasons = await context.ExpediteReasons.Select(x => x.Reason).ToListAsync();
            return reasons ?? new();
        }

        public async Task<List<string>> GetApprovalStatuses()
        {
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            List<string> reasons = await context.ApproverStatusLookups.Select(x => x.StatusName).ToListAsync();
            return reasons ?? new();
        }
    }
}
