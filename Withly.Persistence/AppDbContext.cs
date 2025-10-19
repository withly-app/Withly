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
    public DbSet<Rsvp> Rsvps => Set<Rsvp>();
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
            }
        );

        modelBuilder.Entity<UserProfile>(b =>
            {
                b.Property(up => up.AvatarUrl)
                    .HasMaxLength(500);
            }
        );

        modelBuilder.Entity<ApplicationUser>()
            .HasOne<UserProfile>(user => user.Profile)
            .WithOne(profile => profile.User)
            .HasForeignKey<UserProfile>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invitee>(b =>
            {
                b.HasKey(i => i.Id);
                b.Property(i => i.Email).IsRequired().HasMaxLength(256);
                b.HasIndex(i => i.Email).IsUnique();
                b.Property(i => i.Name).HasMaxLength(100);
            }
        );

        modelBuilder.Entity<Event>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Title).IsRequired().HasMaxLength(100);
                b.Property(e => e.Description).HasMaxLength(500);
                b.Property(e => e.RecurringRule).HasMaxLength(200);
                b.Property(e => e.PublicJoinCode).HasMaxLength(50);

                b.HasMany(e => e.Invitees)
                    .WithMany(i => i.Events)
                    .UsingEntity<Rsvp>(
                        r => r.HasOne(x => x.Invitee)
                            .WithMany(i => i.Rsvps)
                            .HasForeignKey(x => x.InviteeId)
                            .OnDelete(DeleteBehavior.Cascade),
                        r => r.HasOne(x => x.Event)
                            .WithMany(e => e.Rsvps)
                            .HasForeignKey(x => x.EventId)
                            .OnDelete(DeleteBehavior.Cascade),
                        r =>
                        {
                            r.HasKey(x => new { x.EventId, x.InviteeId });
                            r.Property(x => x.Secret).IsRequired().HasMaxLength(256);
                            r.Property(x => x.Status).IsRequired();
                        }
                    );
            }
        );

        modelBuilder.Entity<EmailMessage>()
            .HasOne<ApplicationUser>(email => email.User)
            .WithMany(user => user.Emails)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<EmailMessage>()
            .HasMany(e => e.Attachments)
            .WithMany(a => a.Emails);
    }
}