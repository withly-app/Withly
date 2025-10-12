using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Withly.Persistence.Entities;

namespace Withly.Persistence;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Invitee> Invitees => Set<Invitee>();
    public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            b.Property(u => u.NormalizedEmail)
                .IsRequired()
                .HasMaxLength(256);
        });

        modelBuilder.Entity<UserProfile>(b =>
        {
            b.Property(up => up.AvatarUrl)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<ApplicationUser>()
            .HasOne<UserProfile>(user => user.Profile)
            .WithOne(profile => profile.User)
            .HasForeignKey<UserProfile>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>()
            .HasMany(e => e.Invitees)
            .WithOne(i => i.Event)
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmailMessage>()
            .HasOne<ApplicationUser>(email => email.User)
            .WithMany(user => user.Emails)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<EmailMessage>()
            .HasMany(e => e.Attachments)
            .WithOne(a => a.Email)
            .HasForeignKey(a => a.EmailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}