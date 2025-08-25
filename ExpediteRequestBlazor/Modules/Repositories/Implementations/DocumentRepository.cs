using System.Data;



namespace ExpediteRequestBlazor.Modules.Repositories.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;
        public readonly ILogger<DocumentRepository> _logger;

        public DocumentRepository(IDbContextFactory<ExpediteRequestContext> contextFactory, ILogger<DocumentRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task Create(ExpediteRequestContext context, Document document)
        {
            await context.Documents.AddAsync(document);
            await context.SaveChangesAsync();
        }

        public bool DocumentExists(ExpediteRequestContext context, Document item)
        {
            return context.Documents.Any(doc => doc.Gkey == item.Gkey);
        }

        public async Task Update(ExpediteRequestContext context, Document document)
        {
            context.Documents.Update(document);
            await context.SaveChangesAsync();
        }

        public async Task<Document> Get(ExpediteRequestContext context, Guid Gkey)
        {
            try
            {
                IQueryable<Document> documentQuery = context.Documents.AsQueryable();
                IQueryable<Approval> ApproverQuery = context.Approvals.AsQueryable();
                Document document = await documentQuery.Where(doc => doc.Gkey == Gkey).FirstOrDefaultAsync();
                document.Approvals = ApproverQuery.Where(b => b.DocumentId == document.Id).ToList();

                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return null;
            }
        }


        public IQueryable<ExpediteRequestsExtended> GetAll(ExpediteRequestContext context)
        {
            var result = context.ExpediteRequestsExtended
                .AsNoTracking()
                .Where(d => d.Status != Status.SUBMITTED)
                .AsQueryable();

            return result.AsQueryable();
        }

        public IQueryable<ExpediteRequestsExtended> GetAll_ProductionPlanner(ExpediteRequestContext context)
        {
            var result = context.ExpediteRequestsExtended
                .AsNoTracking()
                .Where(d => d.Status == Status.AWAITING_PRODUCTION)
                .AsQueryable();

            return result.AsQueryable();
        }
    }
}
