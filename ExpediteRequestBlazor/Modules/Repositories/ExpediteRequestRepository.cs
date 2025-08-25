using Dapper;
using ExpediteRequestBlazor.DataTransferObjects;
using ExpediteRequestBlazor.EFModels;
using Indium.Common.Models;
using Indium.Infor.EFContexts;
using Indium.Infor.EFModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ExpediteRequestBlazor.Repositories
{
    public class ExpediteRequestRepository : IExpediteRequestRepository
    {
        private readonly IDbContextFactory<IND_APPContext> _contextSLFactory;
        public readonly ILogger<ExpediteRequestRepository> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string CoitemMstTable = "coitem_mst";
        private const string CoBlnMstTable = "co_bln_mst";
        private const string TrnitemMstTable = "trnitem_mst";
        private const string CoMstTable = "co_mst";
        private const string CustaddrMstTable = "custaddr_mst";
        private const string ItemGlblTable = "item_glbl";
        private const string ItemAllTable = "item_all";
        private const string TransferMstTable = "transfer_mst";

        public ExpediteRequestRepository(IDbContextFactory<IND_APPContext> contextSLFactory, ILogger<ExpediteRequestRepository> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _contextSLFactory = contextSLFactory;
            _logger = logger;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
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

            var lines = await connection.QueryAsync<short>(sql, new { orderNumber });
            return lines.ToList();
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

                    var coItemData = await connection.QueryFirstOrDefaultAsync(sql, new { orderNumber, orderLine, orderRelease });
                    if (coItemData != null)
                    {
                        result.ShipSite = coItemData.ship_site;
                        result.Job = coItemData.ref_num;
                        result.Ipn = coItemData.item;
                        result.QtyOrdered = coItemData.qty_ordered_conv;
                        result.Um = coItemData.u_m;
                        result.DueDate = coItemData.due_date;
                        result.OrderDate = coItemData.order_date;
                        result.IpnDescription = coItemData.ipn_description;
                        result.CustomerName = coItemData.name;
                        result.PlanCode = coItemData.plan_code;
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

                    var trnItemData = await connection.QueryFirstOrDefaultAsync(sqlTrn, new { orderNumber, orderLine });
                    if (trnItemData != null)
                    {
                        result.ShipSite = trnItemData.ship_site;
                        result.Job = trnItemData.ref_num;
                        result.Ipn = trnItemData.item;
                        result.QtyOrdered = trnItemData.qty_ordered_conv;
                        result.Um = trnItemData.u_m;
                        result.DueDate = trnItemData.due_date;
                        result.OrderDate = trnItemData.order_date;
                        result.IpnDescription = trnItemData.ipn_description;
                        result.CustomerName = trnItemData.name;
                        result.PlanCode = trnItemData.plan_code;
                    }
                    break;
            }

            return result;
        }

        public async Task<List<Dropdown>> GetOrderLineDropdownItems(string orderNumber)
        {
            List<Dropdown> result = (await GetOrderLines(orderNumber))
                                    .Select(line => new Dropdown { Id = line.ToString(), Value = line.ToString() }).ToList();

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

            var releases = await connection.QueryAsync<short>(sql, new { orderNumber, orderLine });
            return releases.ToList();
        }

        public async Task<List<Dropdown>> GetOrderReleaseDropdownItems(string orderNumber, short orderLine)
        {
            List<Dropdown> result = (await GetOrderReleases(orderNumber, orderLine))
                                    .Select(release => new Dropdown { Id = release.ToString(), Value = release.ToString() }).ToList();

            return result;
        }

#nullable enable
        public async Task<string> GetOrderType(string orderNumber)
        {
            using var connection = await OpenConnectionAsync();

            string? type = null;

            if (type is null)
            {
                var exists = await connection.ExecuteScalarAsync<int>(
                    $"SELECT CASE WHEN EXISTS (SELECT 1 FROM {TransferMstTable} WHERE trn_num = @orderNumber) THEN 1 ELSE 0 END",
                    new { orderNumber });

                if (exists == 1)
                    type = "TRN";
            }
            if (type is null)
            {
                type = await connection.QueryFirstOrDefaultAsync<string>(
                    $"SELECT TOP 1 Type FROM {CoMstTable} WHERE co_num = @orderNumber AND site_ref = 'MASTER'",
                    new { orderNumber });
            }
            _ = type ?? throw new ArgumentException("Order number was not found. Please check syteline", nameof(orderNumber));
            return type;
        }
#nullable disable

        public async Task<ExpandKyResult> ExpandKy(int keyLen, string key)
        {
            using var connection = await OpenConnectionAsync();
            return await connection.QueryFirstAsync<ExpandKyResult>("SELECT dbo.ExpandKy(@keyLen, @key) AS Value", new { keyLen, key });
        }
    }
}

