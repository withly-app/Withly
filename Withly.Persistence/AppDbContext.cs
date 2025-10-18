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
    public DbSet<EmailAttachment> EmailAttachments => Set<EmailAttachment>();

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
            .WithMany(i => i.Events);

        modelBuilder.Entity<Invitee>(b =>
            {
                b.Property(i => i.Email)
                    .IsRequired();

                b.HasIndex(i => i.Email)
                    .IsUnique();
            }
        );
        
        modelBuilder.Entity<Invitee>()
            .HasMany(i => i.Rsvps)
            .WithOne(r => r.Invitee)
            .HasForeignKey(r => r.InviteeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rsvp>()
            .HasKey(r => new { r.EventId, r.InviteeId });
        
        modelBuilder.Entity<Rsvp>()
            .HasOne<Event>(e => e.Event)
            .WithMany(ev => ev.Rsvps)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmailMessage>()
            .HasOne<ApplicationUser>(email => email.User)
            .WithMany(user => user.Emails)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<EmailMessage>()
            .HasMany(e => e.Attachments)
            .WithMany(a => a.Emails);
    }
}