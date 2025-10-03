

using System.Linq.Expressions;

namespace ExpediteRequestBlazor.Modules.Repositories.Interfaces
{
    public interface IDocumentRepository
    {
        bool DocumentExists(ExpediteRequestContext context, Document item);
        IQueryable<ExpediteRequestsExtended> GetAll(ExpediteRequestContext context);
        IQueryable<ExpediteRequestsExtended> GetAll_ProductionPlanner(ExpediteRequestContext context);
        Task<List<ExpediteRequestsExtended>> GetBy_RequestExtended(ExpediteRequestContext context, Expression<Func<ExpediteRequestsExtended, bool>> selector);
        Task<List<string>> GetAll_SitesFromRequests(ExpediteRequestContext context);
        Task<List<string>> GetAll_ExpediteStatusFromRequests(ExpediteRequestContext context);
        Task<List<string>> GetAll_PlannerCodeFromRequests(ExpediteRequestContext context);
        Task<List<string>> GetAll_SitesFromProductionPlanner(ExpediteRequestContext context);
        Task<List<string>> GetAll_ExpediteStatusFromProductionPlanner(ExpediteRequestContext context);
        Task<List<string>> GetAll_PlannerCodeFromProductionPlanner(ExpediteRequestContext context);
        Task Update(ExpediteRequestContext context, Document document);
        Task<Document> Get(ExpediteRequestContext context, Guid Gkey);
        Task Create(ExpediteRequestContext context, Document document);
    }
}
