using System;
using System.Threading.Tasks;
using ExpediteRequestBlazor.EFModels;
using ExpediteRequestBlazor.DataTransferObjects;
using ExpediteRequestBlazor.Models;
using Indium.Common.DataTransferObjects;
using ExpediteRequestBlazor.EFModels.ViewModels;

namespace ExpediteRequestBlazor.Repositories
{
    public interface IDocumentRepository
    {
        Task SaveObject(Document document);
        bool DocumentExists(Document item);
        void ValidateDocumentExists(Document item);
        bool DocumentExists(DocumentModel item);
        Task HandleObject(Document document);
        Task UpdateObjectStatus(Document document, ElsaDocument elsaDocument);
        Task<Document> RetrieveObject(Guid Gkey);
        IQueryable<ExpediteRequestsExtended> GetDocuments();
        IQueryable<ExpediteRequestsExtended> GetProductionPlannerDocuments();

    }
}
