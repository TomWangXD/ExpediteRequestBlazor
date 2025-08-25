namespace ExpediteRequestBlazor.Shared
{

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components.Authorization;
    using System.Linq;
    using System.Runtime.Versioning;

    public partial class User : Indium.Common.Models.User
    {
        [SupportedOSPlatform("windows")]
        public User(IDbContextFactory<CommonContext> CommonDBFactory, Indium.Common.Models.User user, IConfiguration config, AuthenticationStateProvider authProvider, ILogger<User> logger, IAuthorizationService authorizationService)
        : base(authProvider, CommonDBFactory, logger, authorizationService)
        {
            _CommonDBFactory = CommonDBFactory;
            UserTitle = GetUserTitle(user, config);
        }

        public IDbContextFactory<CommonContext> _CommonDBFactory;

        public string UserTitle { get; set; } = string.Empty;

        public string GetUserTitle(Indium.Common.Models.User user, IConfiguration config)
        {
            var authorizationGroups = config.GetSection("AuthorizationPolicyGroups");

            List<string?> userTitles = new();

            foreach (var group in authorizationGroups.GetChildren())
            {
                string? groupValue = group.Value;

                if (user.Groups is not null)
                {
                    if (user.Groups.Any(x => x == group.Value))
                    {
                        userTitles.Add(group.Key);
                    }
                }
            }

            return string.Join(',', userTitles);
        }
    }
}
