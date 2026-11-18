using KN.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KN.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<KazaPeriod> KazaPeriods => Set<KazaPeriod>();
    public DbSet<KazaLog>  KazaLogs => Set<KazaLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<KazaPeriod>(e =>
        {
            e.HasIndex(x => x.UserId);
            e.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<KazaLog>(e =>
            {
                e.HasIndex(x => new { x.UserId, x.PrayerType });
                e.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });
    }
}

