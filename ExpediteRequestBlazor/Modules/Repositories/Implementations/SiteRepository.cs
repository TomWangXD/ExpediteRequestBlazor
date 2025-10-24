
using System.Linq.Expressions;
using ExpediteRequestBlazor.Modules.Repositories.Interfaces;
using Indium.Common.EFModels;
using Indium.Infor.EFContexts;
using Microsoft.EntityFrameworkCore.Internal;
using static Azure.Core.HttpHeader;

namespace ExpediteRequestBlazor.Modules.Repositories.Implementations
{
    public class SiteRepository : ISiteRepository
    {
        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;
        
        // will delete after refactoring request repo
        public CmSiteMaster? GetByNA(Expression<Func<CmSiteMaster, bool>> selector, CommonContext commonContext, IND_APPContext indContext)
        {
            CmSiteMaster? value = commonContext.CmSiteMasters.Where(selector).OrderBy(x => x.SiteCode).FirstOrDefault();

            if (value is null)
            {
                return null;
            }

            SiteGroup? siteGroup = indContext.SiteGroups.Where(x => x.SiteGroup1 == "DP" && x.Site == value.SiteCode).FirstOrDefault();

            if (siteGroup?.Site == value?.SiteCode)
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public async Task<CmSiteMaster?> GetBy(Expression<Func<CmSiteMaster, bool>> selector, CommonContext commonContext, IND_APPContext indContext)
        {
            CmSiteMaster? value = await commonContext.CmSiteMasters.Where(selector).OrderBy(x => x.SiteCode).FirstOrDefaultAsync();

            if (value is null)
            {
                return null;
            }

            SiteGroup? siteGroup = indContext.SiteGroups.Where(x => x.SiteGroup1 == "DP" && x.Site == value.SiteCode).FirstOrDefault();

            if (siteGroup?.Site == value?.SiteCode)
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        // will delete after refactoring request repo

        public CmSiteMaster? GetSiteByNA(Expression<Func<CmSiteMaster, bool>> selector, CommonContext context)
        {
            CmSiteMaster? site = context.CmSiteMasters.Where(selector).OrderBy(x => x.SiteCode).FirstOrDefault();
            return site;
        }

        public async Task<CmSiteMaster?> GetSiteBy(Expression<Func<CmSiteMaster, bool>> selector, CommonContext context)
        {
            CmSiteMaster? site = await context.CmSiteMasters.Where(selector).OrderBy(x => x.SiteCode).FirstOrDefaultAsync();
            return site;
        }

        public async Task<string?> GetSiteBySiteId(CommonContext commonContext, int? siteId = null)
        {
             var result = commonContext.CmSiteMasters
            .AsNoTracking()
            .Where(x => x.Id == siteId)
            .Select(x => x.SiteCode)   
            .FirstOrDefault();
                       
            return result;
        }


        public SiteGroup? GetSiteGroupsByNA(Expression<Func<SiteGroup, bool>> selector, IND_APPContext appContext)
        {
            SiteGroup? siteGroup = appContext.SiteGroups.Where(selector).FirstOrDefault();
            return siteGroup;
        }

        public async Task<SiteGroup?> GetSiteGroupsBy(Expression<Func<SiteGroup, bool>> selector, IND_APPContext appContext)
        {
            SiteGroup? siteGroup = await appContext.SiteGroups.Where(selector).FirstOrDefaultAsync();
            return siteGroup;
        }

        public async Task<List<CmSiteMaster>> GetAll(CommonContext context, IND_APPContext appContext)
        {
            List<CmSiteMaster> sites = await context.CmSiteMasters
                .OrderBy(x => x.SiteCode)
                .AsNoTracking()
                .ToListAsync();

            return sites;
        }

        public async Task<List<CmSiteMaster>> GetLimitedListBy(Expression<Func<CmSiteMaster, bool>> selector, int take, CommonContext context, IND_APPContext appContext)
        {
            List<CmSiteMaster> sites = await context.CmSiteMasters.Where(selector).OrderBy(x => x.SiteCode).Take(take).ToListAsync();
            return sites;
        }

        public async Task<List<CmSiteMaster>> GetListBy(Expression<Func<CmSiteMaster, bool>> selector, CommonContext context, IND_APPContext appContext)
        {
            List<CmSiteMaster> sites = await context.CmSiteMasters.Where(selector).OrderBy(x => x.SiteCode).AsNoTracking().ToListAsync();
            return sites;
        }

        public async Task<List<CmSiteMaster>> GetFilteredSites(Expression<Func<SiteGroup, bool>> selector, List<CmSiteMaster> sites, IND_APPContext appContext)
        {
            var siteGroups = await appContext.SiteGroups
            .Where(selector)
            .AsNoTracking()
            .ToListAsync();

            var matchingSiteCodes = siteGroups.Select(sg => sg.Site).ToHashSet();
            var result = sites.Where(site => matchingSiteCodes.Contains(site.SiteCode)).ToList();

            return result;
        }

        public CmSiteMaster? GetNA(int key, CommonContext context, IND_APPContext appContext)
        {
            CmSiteMaster? value = context.CmSiteMasters.Where(x => x.Id == key).FirstOrDefault();

            if (value is null)
            {
                return null;
            }

            SiteGroup? siteGroup = appContext.SiteGroups.Where(x => x.SiteGroup1 == "DP" && x.Site == value.SiteCode).FirstOrDefault();
            if (siteGroup?.Site == value?.SiteCode)
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}