using ExpediteRequestBlazor.EFModels;
using ExpediteRequestBlazor.EFModels.ViewModels;
using System.Data;
using System.Linq.Expressions;



namespace ExpediteRequestBlazor.Modules.Repositories.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {

        public DocumentRepository()
        {

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
            IQueryable<Document> documentQuery = context.Documents.AsQueryable();
            IQueryable<Approval> ApproverQuery = context.Approvals.AsQueryable();
            Document document = await documentQuery.Where(doc => doc.Gkey == Gkey).FirstOrDefaultAsync();
            document.Approvals = ApproverQuery.Where(b => b.DocumentId == document.Id).ToList();
            return document;
        }

        public async Task<List<ExpediteRequestsExtended>> GetBy_RequestExtended(ExpediteRequestContext context, Expression<Func<ExpediteRequestsExtended, bool>> selector)
        {
            return await context.ExpediteRequestsExtended.Where(selector).ToListAsync();
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

        public async Task<List<string>> GetAll_SitesFromRequests(ExpediteRequestContext context)
        {
            return await context.Documents
                .Where(x => !string.IsNullOrEmpty(x.ShipSite))
                .Select(x => x.ShipSite.Trim().ToUpper())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}
