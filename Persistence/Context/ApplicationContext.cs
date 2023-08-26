using Domain.Entities;
using Domain.Enums;
using Persistence.Configs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Configs(modelBuilder);
    }
    private void Configs(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApplicationRoleConfig());
    }

}