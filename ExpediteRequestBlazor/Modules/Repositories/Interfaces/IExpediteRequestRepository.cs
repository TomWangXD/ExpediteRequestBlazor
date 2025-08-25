using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ExpediteRequestBlazor.DataTransferObjects;
using ExpediteRequestBlazor.Models;
using Indium.Infor.EFModels;

namespace ExpediteRequestBlazor.Modules.Repositories.Interfaces
{
    public interface IExpediteRequestRepository
    {
        Task<List<short>> GetOrderLines(IDbConnection connection, string sql, string orderNumber);
        Task<SytelineDocumentData> GetCoItemData(IDbConnection connection, string sql, SytelineDocumentData result, string orderNumber, short orderLine, short orderRelease);
        Task<SytelineDocumentData> GetTrnItemData(IDbConnection connection, string sql, SytelineDocumentData result, string orderNumber, short orderLine);
        Task<List<short>> GetOrderReleases(IDbConnection connection, string sql, string orderNumber, short orderLine);
        Task<string> GetOrderType(string orderNumber);
        Task<bool> TypeExists_TransferMst(IDbConnection connection, string orderNumber);
        Task<string> TypeExists_CoMst(IDbConnection connection, string orderNumber);
        Task<ExpandKyResult> ExpandKy(IDbConnection connection, int keyLen, string key);
    }
}
