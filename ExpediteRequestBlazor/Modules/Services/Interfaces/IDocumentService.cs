using ExpediteRequestBlazor.DataTransferObjects;

namespace ExpediteRequestBlazor.Modules.Services.Interfaces
{
    public interface IDocumentService
    {
        IQueryable<ExpediteRequestsExtended> GetAll_ProductionPlanner();
        IQueryable<ExpediteRequestsExtended> GetAll();

        Task HandleObject(Document document);
        void ValidateDocumentExists(Document item);
        Task UpdateObjectStatus(Document document, ElsaDocument elsaDocument);
        Task<T> ParseWorkflowEngineResponseAsync<T>(HttpContent content);
        Task<HttpContent> GetWorkflowEngineResponseAsync<T>(T item);
        Task<Document> Get(Guid Gkey);
    }
}
