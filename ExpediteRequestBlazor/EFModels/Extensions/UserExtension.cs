namespace ExpediteRequestBlazor.EFModels.Extensions
{
    public partial class User
    {
        public User(IDbContextFactory<CommonContext> CommonDBFactory, Indium.Common.Models.User user)
        {
            _CommonDBFactory = CommonDBFactory;
            UserSite = GetUserSite(user);
        }

        public IDbContextFactory<CommonContext> _CommonDBFactory;
        public int? UserSite { get; set; }
        public bool IsAdmin { get; set; }


        public int? GetUserSite(Indium.Common.Models.User user)
        {
            CommonContext commonContext = _CommonDBFactory.CreateDbContext();

            if (user.Employee is not null)
            {
                return commonContext.CmEmployeeMasters.Where(x => x.SamaccountName == user.Employee.SamaccountName && x.ActiveForDisplay == true).Select(x => x.SiteId).FirstOrDefault();
            }
            return null;
        }
    }
}
