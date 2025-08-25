using System;
using System.Threading.Tasks;
using ExpediteRequestBlazor.EFModels;
using ExpediteRequestBlazor.DataTransferObjects;
using ExpediteRequestBlazor.Models;
using Indium.Common.DataTransferObjects;
using ExpediteRequestBlazor.EFModels.ViewModels;

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
