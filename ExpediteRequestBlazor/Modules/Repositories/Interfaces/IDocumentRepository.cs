

namespace ExpediteRequestBlazor.Modules.Repositories.Interfaces
{
    public interface IDocumentRepository
    {
        bool DocumentExists(ExpediteRequestContext context, Document item);
        IQueryable<ExpediteRequestsExtended> GetAll(ExpediteRequestContext context);
        IQueryable<ExpediteRequestsExtended> GetAll_ProductionPlanner(ExpediteRequestContext context);
        Task Update(ExpediteRequestContext context, Document document);
        Task<Document> Get(ExpediteRequestContext context, Guid Gkey);
        Task Create(ExpediteRequestContext context, Document document);
    }
}
