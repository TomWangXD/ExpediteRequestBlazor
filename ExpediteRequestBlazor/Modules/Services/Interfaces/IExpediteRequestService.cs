using ExpediteRequestBlazor.DataTransferObjects;
using System.Data;

namespace ExpediteRequestBlazor.Modules.Services.Interfaces
{
    public interface IExpediteRequestService
    {
        Task<List<short>> GetOrderLines(string orderNumber);
        Task<string> GetOrderType(string orderNumber);
        Task<SytelineDocumentData> GetSytelineDocumentDataAsync(string orderNumber, short orderLine, short orderRelease);
        Task<List<short>> GetOrderReleases(string orderNumber, short orderLine);
        Task<ExpandKyResult> ExpandKy(int keyLen, string key);
    }
}
