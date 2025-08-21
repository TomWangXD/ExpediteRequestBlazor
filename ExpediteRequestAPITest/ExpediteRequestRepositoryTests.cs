using ExpediteRequestAPI.Repositories;
using Indium.Common.Models;
using Indium.Infor.EFContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExpediteRequestAPITest
{
    [TestClass]
    public class ExpediteRequestRepositoryTests
    {
        private class FakeExpediteRequestRepository : ExpediteRequestRepository
        {
            public FakeExpediteRequestRepository() : base(new DummyFactory(), LoggerFactory.Create(builder => { }).CreateLogger<ExpediteRequestRepository>(), new ConfigurationBuilder().Build(), new HttpContextAccessor()) { }

            public override Task<List<short>> GetOrderLines(string orderNumber)
            {
                return Task.FromResult(new List<short> { 1, 2, 3 });
            }

            public override Task<List<short>> GetOrderReleases(string orderNumber, short orderLine)
            {
                return Task.FromResult(new List<short> { 0, 1 });
            }
        }

        private class DummyFactory : IDbContextFactory<IND_APPContext>
        {
            public IND_APPContext CreateDbContext()
            {
                throw new System.NotImplementedException();
            }

            public ValueTask<IND_APPContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
            {
                return new ValueTask<IND_APPContext>(CreateDbContext());
            }
        }

        [TestMethod]
        public async Task GetOrderLineDropdownItems_ReturnsDropdowns()
        {
            var repo = new FakeExpediteRequestRepository();
            List<Dropdown> result = await repo.GetOrderLineDropdownItems("ORD1");

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("1", result[0].Id);
            Assert.AreEqual("2", result[1].Value);
        }

        [TestMethod]
        public async Task GetOrderReleaseDropdownItems_ReturnsDropdowns()
        {
            var repo = new FakeExpediteRequestRepository();
            List<Dropdown> result = await repo.GetOrderReleaseDropdownItems("ORD1", 1);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("0", result[0].Id);
            Assert.AreEqual("1", result[1].Value);
        }
    }
}

