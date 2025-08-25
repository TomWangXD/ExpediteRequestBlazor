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
using ExpediteRequestBlazor.Modules.Repositories.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ExpediteRequestBlazor.Modules.Repositories.Implementations
{
    public class ExpediteRequestRepository : IExpediteRequestRepository
    {
        private readonly IDbContextFactory<IND_APPContext> _contextSLFactory;
        public readonly ILogger<ExpediteRequestRepository> _logger;


        private const string CoMstTable = "co_mst";
        private const string TransferMstTable = "transfer_mst";

        public ExpediteRequestRepository(IDbContextFactory<IND_APPContext> contextSLFactory, ILogger<ExpediteRequestRepository> logger)
        {
            _contextSLFactory = contextSLFactory;
            _logger = logger;
        }

        private async Task<IDbConnection> OpenConnectionAsync()
        {
            await using IND_APPContext contextSL = await _contextSLFactory.CreateDbContextAsync();
            var connection = new SqlConnection(contextSL.Database.GetConnectionString());
            await connection.OpenAsync();
            return connection;
        }

        public virtual async Task<List<short>> GetOrderLines(IDbConnection connection, string sql, string orderNumber)
        {
            var lines = await connection.QueryAsync<short>(sql, new { orderNumber });
            return lines.ToList();
        }


        public async Task<SytelineDocumentData>GetCoItemData(IDbConnection connection, string sql, SytelineDocumentData result, string orderNumber, short orderLine, short orderRelease)
        {
            var coItemData = await connection.QueryFirstOrDefaultAsync(sql, new { orderNumber, orderLine, orderRelease });
            return coItemData;
        }

        public async Task<SytelineDocumentData> GetTrnItemData(IDbConnection connection, string sql, SytelineDocumentData result, string orderNumber, short orderLine)
        {
            var coItemData = await connection.QueryFirstOrDefaultAsync(sql, new { orderNumber, orderLine});
            return coItemData;
        }


        public virtual async Task<List<short>> GetOrderReleases(IDbConnection connection, string sql, string orderNumber, short orderLine)
        {
            var releases = await connection.QueryAsync<short>(sql, new { orderNumber, orderLine });
            return releases.ToList();
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

        public async Task<bool> TypeExists_TransferMst(IDbConnection connection, string orderNumber)
        {
            int exists = await connection.ExecuteScalarAsync<int>(
                        $"SELECT CASE WHEN EXISTS (SELECT 1 FROM {TransferMstTable} WHERE trn_num = @orderNumber) THEN 1 ELSE 0 END",
                        new { orderNumber });
            return exists == 1;
        }

        public async Task<string> TypeExists_CoMst(IDbConnection connection, string orderNumber)
        {
            return await connection.QueryFirstOrDefaultAsync<string>(
                        $"SELECT TOP 1 Type FROM {CoMstTable} WHERE co_num = @orderNumber AND site_ref = 'MASTER'",
                        new { orderNumber });
        }

#nullable disable

        public async Task<ExpandKyResult> ExpandKy(IDbConnection connection, int keyLen, string key)
        {
            return await connection.QueryFirstAsync<ExpandKyResult>("SELECT dbo.ExpandKy(@keyLen, @key) AS Value", new { keyLen, key });
        }
    }
}

