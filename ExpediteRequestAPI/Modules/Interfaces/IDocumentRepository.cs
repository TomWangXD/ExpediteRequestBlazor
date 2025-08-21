using System;
using System.Threading.Tasks;
using ExpediteRequestAPI.EFModels;
using ExpediteRequestAPI.DataTransferObjects;
using ExpediteRequestAPI.Models;
using Indium.Common.DataTransferObjects;
using ExpediteRequestAPI.EFModels.ViewModels;

namespace ExpediteRequestAPI.Repositories
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
    }
}
