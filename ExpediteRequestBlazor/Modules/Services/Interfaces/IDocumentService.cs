using ExpediteRequestBlazor.DataTransferObjects;
using System.Linq.Expressions;

namespace ExpediteRequestBlazor.Modules.Services.Interfaces
{
    public interface IDocumentService
    {
        IQueryable<ExpediteRequestsExtended> GetAll_ProductionPlanner();
        IQueryable<ExpediteRequestsExtended> GetAll();
        Task<List<ExpediteRequestsExtended>> GetAll_Export(DateRange? dateRange = null);
        Task<List<ExpediteRequestsExtended>> GetBy_RequestExtended(Expression<Func<ExpediteRequestsExtended, bool>> selector);
        Task HandleObject(Document document);
        void ValidateDocumentExists(Document item);
        Task DownloadExcelFile(List<ExpediteRequestsExtended> data, IJSRuntime jsRuntime, ILogger logger);
        Task UpdateObjectStatus(Document document, ElsaDocument elsaDocument);
        Task<T> ParseWorkflowEngineResponseAsync<T>(HttpContent content);
        Task<HttpContent> GetWorkflowEngineResponseAsync<T>(T item);
        Task<Document> Get(Guid Gkey);
    }
}
