using System.Collections.Generic;
using System.Threading.Tasks;
using ExpediteRequestBlazor.DataTransferObjects;
using ExpediteRequestBlazor.Models;
using Indium.Infor.EFModels;

namespace ExpediteRequestBlazor.Repositories
{
    public interface IExpediteRequestRepository
    {
        Task<List<short>> GetOrderLines(string orderNumber);
        Task<SytelineDocumentData> GetSytelineDocumentDataAsync(string orderNumber, short orderLine, short orderRelease);
        Task<List<Dropdown>> GetOrderLineDropdownItems(string orderNumber);
        Task<List<short>> GetOrderReleases(string orderNumber, short orderLine);
        Task<List<Dropdown>> GetOrderReleaseDropdownItems(string orderNumber, short orderLine);
        Task<string> GetOrderType(string orderNumber);
        Task<ExpandKyResult> ExpandKy(int keyLen, string key);
    }
}
