
using System.Linq.Expressions;
using Indium.Infor.EFContexts;
using Indium.Common.EFModels;

namespace ExpediteRequestBlazor.Modules.Repositories.Implementations
{
    public interface ISiteRepository
    {
        Task<CmSiteMaster?> GetBy(Expression<Func<CmSiteMaster, bool>> selector, CommonContext commonContext, IND_APPContext indContext);

        CmSiteMaster? GetNA(int key, CommonContext context, IND_APPContext appContext);

        CmSiteMaster? GetByNA(Expression<Func<CmSiteMaster, bool>> selector, CommonContext commonContext, IND_APPContext indContext);

        Task<CmSiteMaster?> GetSiteBy(Expression<Func<CmSiteMaster, bool>> selector, CommonContext context);

        //Task<CmSiteMaster?> GetSiteBySiteId(CommonContext commonContext, int? siteId = null);
        Task<string?> GetSiteBySiteId(CommonContext commonContext, int? siteId = null);

        SiteGroup? GetSiteGroupsByNA(Expression<Func<SiteGroup, bool>> selector, IND_APPContext appContext);
        Task<SiteGroup?> GetSiteGroupsBy(Expression<Func<SiteGroup, bool>> selector, IND_APPContext appContext);
        CmSiteMaster? GetSiteByNA(Expression<Func<CmSiteMaster, bool>> selector, CommonContext context);

        Task<List<CmSiteMaster>> GetAll(CommonContext context, IND_APPContext appContext);
        Task<List<CmSiteMaster>> GetLimitedListBy(Expression<Func<CmSiteMaster, bool>> selector, int take, CommonContext context, IND_APPContext appContext);
        Task<List<CmSiteMaster>> GetListBy(Expression<Func<CmSiteMaster, bool>> selector, CommonContext context, IND_APPContext appContext);
        Task<List<CmSiteMaster>> GetFilteredSites(Expression<Func<SiteGroup, bool>> selector, List<CmSiteMaster> sites, IND_APPContext appContext);
    }
}