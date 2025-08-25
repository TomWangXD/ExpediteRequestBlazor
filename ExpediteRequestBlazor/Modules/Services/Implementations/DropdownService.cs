namespace ExpediteRequestBlazor.Modules.Services.Implementations
{
    public class DropdownService : IDropdownService
    {

        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;
        private readonly IDropdownRepository _dropdownRepository;
        public readonly ILogger<DocumentRepository> _logger;

        public DropdownService(IDbContextFactory<ExpediteRequestContext> contextFactory, ILogger<DocumentRepository> logger, IDropdownRepository dropdownRepository)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _dropdownRepository = dropdownRepository;
        }

        public async Task<List<string>> GetExpediteReasons()
        {
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            return await _dropdownRepository.GetExpediteReasons(context);
;
        }

        public async Task<List<string>> GetApprovalStatuses()
        {
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            return await _dropdownRepository.GetApprovalStatuses(context);
        }
    }
}
