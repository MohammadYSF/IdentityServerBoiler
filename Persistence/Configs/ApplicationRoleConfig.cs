using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Persistence.Configs
{
    public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData(
            new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = Enum.GetName(typeof(Role), Role.User),
                NormalizedName = Enum.GetName(typeof(Role), Role.User).ToUpper()
            },           
            new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = Enum.GetName(typeof(Role), Role.Admin),
                NormalizedName = Enum.GetName(typeof(Role), Role.Admin).ToUpper()

            }
            );
        }
    }
}
  