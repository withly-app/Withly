using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Withly.Domain.Entities;
using Withly.Infrastructure.Auth;

namespace Withly.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Invitee> Invitees => Set<Invitee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>()
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<UserProfile>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Invitees)
            .WithOne(i => i.Event)
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
