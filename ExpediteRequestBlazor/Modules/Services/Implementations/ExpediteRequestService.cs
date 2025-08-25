using ExpediteRequestBlazor.DataTransferObjects;
using Indium.Infor.EFContexts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ExpediteRequestBlazor.Modules.Services.Implementations
{
    public class ExpediteRequestService : IExpediteRequestService
    {

            private readonly IDbContextFactory<IND_APPContext> _contextSLFactory;
            private readonly IExpediteRequestRepository _expediteRequestRepository;
            public readonly ILogger<ExpediteRequestService> _logger;

            private const string CoitemMstTable = "coitem_mst";
            private const string CoBlnMstTable = "co_bln_mst";
            private const string TrnitemMstTable = "trnitem_mst";
            private const string CoMstTable = "co_mst";
            private const string CustaddrMstTable = "custaddr_mst";
            private const string ItemGlblTable = "item_glbl";
            private const string ItemAllTable = "item_all";
            private const string TransferMstTable = "transfer_mst";

            public ExpediteRequestService(IDbContextFactory<IND_APPContext> contextSLFactory, ILogger<ExpediteRequestService> logger, IExpediteRequestRepository expediteRequestRepository)
            {
                _contextSLFactory = contextSLFactory;
                _logger = logger;
                _expediteRequestRepository = expediteRequestRepository;
            }

            private async Task<IDbConnection> OpenConnectionAsync()
            {
                await using IND_APPContext contextSL = await _contextSLFactory.CreateDbContextAsync();
                var connection = new SqlConnection(contextSL.Database.GetConnectionString());
                await connection.OpenAsync();
                return connection;
            }

            public virtual async Task<List<short>> GetOrderLines(string orderNumber)
            {
                orderNumber = (await ExpandKy(10, orderNumber))?.Value;

                string orderType = await GetOrderType(orderNumber);
                using var connection = await OpenConnectionAsync();

                string sql = orderType switch
                {
                    "R" => $"SELECT co_line FROM {CoitemMstTable} WHERE Co_Num = @orderNumber AND Site_Ref = Ship_Site AND Stat = 'O'",
                    "B" => $"SELECT co_line FROM {CoBlnMstTable} WHERE Co_Num = @orderNumber AND Site_Ref = Ship_Site AND Stat = 'O'",
                    "TRN" => $"SELECT trn_line FROM {TrnitemMstTable} WHERE trn_num = @orderNumber AND site_ref = from_site AND stat = 'O'",
                    _ => string.Empty
                };

                if (string.IsNullOrWhiteSpace(sql))
                {
                    return new List<short>();
                }
                return await _expediteRequestRepository.GetOrderLines(connection, orderNumber, sql);
            }

            public async Task<SytelineDocumentData> GetSytelineDocumentDataAsync(string orderNumber, short orderLine, short orderRelease)
            {
                orderNumber = (await ExpandKy(10, orderNumber))?.Value;

                SytelineDocumentData result = new()
                {
                    CoNum = orderNumber,
                    CoLine = orderLine,
                    CoRelease = orderRelease
                };

                string orderType = await GetOrderType(orderNumber);
                using var connection = await OpenConnectionAsync();

                switch (orderType)
                {
                    case "B":
                    case "R":
                        string ci = CoitemMstTable;
                        string co = CoMstTable;
                        string ca = CustaddrMstTable;
                        string ig = ItemGlblTable;
                        string ia = ItemAllTable;

                        string sql = $@"SELECT ci.ship_site, ci.ref_num, ci.item, ci.qty_ordered_conv, ci.u_m, ci.due_date, co.order_date, ca.name,
                                       ig.description AS ipn_description, ia.plan_code
                                   FROM {ci} ci
                                   JOIN {co} co ON ci.site_ref = co.Site_ref AND ci.Co_Num = co.co_num
                                   JOIN {ca} ca ON co.site_ref = ca.site_ref AND co.cust_num = ca.cust_num AND ca.cust_seq = 0
                                   LEFT JOIN {ig} ig ON ig.item = ci.item
                                   LEFT JOIN {ia} ia ON ia.site_ref = ci.ship_site AND ia.item = ci.item
                                   WHERE ci.Co_Num = @orderNumber AND ci.Co_Line = @orderLine AND ci.Co_Release = @orderRelease AND ci.Site_Ref = ci.Ship_Site";


                    var coItemData = await _expediteRequestRepository.GetCoItemData(connection, sql, result, orderNumber, orderLine, orderRelease);

                        if (coItemData != null)
                        {
                            result.ShipSite = coItemData.ShipSite;
                            result.Job = coItemData.Job;
                            result.Ipn = coItemData.Ipn;
                            result.QtyOrdered = coItemData.QtyOrdered;
                            result.Um = coItemData.Um;
                            result.DueDate = coItemData.DueDate;
                            result.OrderDate = coItemData.OrderDate;
                            result.IpnDescription = coItemData.IpnDescription;
                            result.CustomerName = coItemData.CustomerName;
                            result.PlanCode = coItemData.PlanCode;
                        }
                        break;

                    case "TRN":
                        string ti = TrnitemMstTable;
                        string to = TransferMstTable;
                        ca = CustaddrMstTable;
                        ig = ItemGlblTable;
                        ia = ItemAllTable;

                        string sqlTrn = $@"SELECT ti.ship_site, ti.Frm_ref_num, ti.item, ti.qty_ordered_conv, ti.u_m, ti.Sch_ship_date, toM.order_date, ti.cust_num,
                                             ig.Description AS ipn_description, ca.name, ia.plan_code
                                      FROM {ti} ti
                                      JOIN {to} toM ON ti.site_ref = toM.site_ref AND ti.trn_num = toM.trn_num
                                      LEFT JOIN {ig} ig ON ig.item = ti.item
                                      LEFT JOIN {ca} ca ON ca.site_ref = ti.from_site AND ca.cust_num = ti.cust_num AND ca.cust_seq = 0
                                      LEFT JOIN {ia} ia ON ia.site_ref = ti.from_site AND ia.item = ti.item
                                      WHERE ti.trn_num = @orderNumber AND ti.trn_line = @orderLine AND ti.site_ref = ti.from_site";

                        var trnItemData = await _expediteRequestRepository.GetTrnItemData(connection, sqlTrn, result, orderNumber, orderLine);

                        if (trnItemData != null)
                        {
                            result.ShipSite = trnItemData.ShipSite;
                            result.Job = trnItemData.Job;
                            result.Ipn = trnItemData.Ipn;
                            result.QtyOrdered = trnItemData.QtyOrdered;
                            result.Um = trnItemData.Um;
                            result.DueDate = trnItemData.DueDate;
                            result.OrderDate = trnItemData.OrderDate;
                            result.IpnDescription = trnItemData.IpnDescription;
                            result.CustomerName = trnItemData.CustomerName;
                            result.PlanCode = trnItemData.PlanCode;
                        }
                        break;
                }

                return result;
            }

            public virtual async Task<List<short>> GetOrderReleases(string orderNumber, short orderLine)
            {
                orderNumber = (await ExpandKy(10, orderNumber))?.Value;

                string orderType = await GetOrderType(orderNumber);
                if (orderType != "B")
                {
                    return new List<short> { 0 };
                }

                using var connection = await OpenConnectionAsync();
                string table = CoitemMstTable;
                string sql = $"SELECT Co_Release FROM {table} WHERE Co_Num = @orderNumber AND Co_Line = @orderLine AND Site_Ref = Ship_Site";

                var releases = await _expediteRequestRepository.GetOrderReleases(connection, sql, orderNumber, orderLine);
                return releases.ToList();
            }

            public async Task<string> GetOrderType(string orderNumber)
            {
                using var connection = await OpenConnectionAsync();

                string? type = null;

                if (type is null)
                {
                var exists = await _expediteRequestRepository.TypeExists_TransferMst(connection, orderNumber);

                    if (exists)
                        type = "TRN";
                }
                if (type is null)
                {
                    type = await _expediteRequestRepository.TypeExists_CoMst(connection, orderNumber);
                }
                _ = type ?? throw new ArgumentException("Order number was not found. Please check syteline", nameof(orderNumber));
                return type;
            }

            public async Task<ExpandKyResult> ExpandKy(int keyLen, string key)
            {
                using var connection = await OpenConnectionAsync();
                return await _expediteRequestRepository.ExpandKy(connection, keyLen, key);
            }
        }
}
