using ExpediteRequestBlazor.EFModels.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;

namespace ExpediteRequestBlazor.EFModels.Configurations
{
    public class ExpediteRequestExtendedConfiguration
    {
        public void Configure(EntityTypeBuilder<ExpediteRequestsExtended> builder)
        {
            builder.ToView(nameof(ExpediteRequestsExtended))
                .HasKey(x => x.Id);
        }
    }
}
