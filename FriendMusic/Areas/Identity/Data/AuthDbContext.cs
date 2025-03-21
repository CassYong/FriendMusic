using FriendMusic.Areas.Identity.Data;
using FriendMusic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FriendMusic.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<Music> Music { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Friendship> Friendship { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Music entity
            builder.Entity<Music>(entity =>
            {
                entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Artist).HasMaxLength(200);
                entity.Property(m => m.Album).HasMaxLength(200);
                entity.Property(m => m.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(m => m.UploadDate).IsRequired();
            });

            // Configure Artist entity
            builder.Entity<Artist>(entity =>
            {
                entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Biography).IsRequired();
                entity.HasMany(a => a.Albums)
                      .WithOne(a => a.Artist)
                      .HasForeignKey(a => a.ArtistId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); // or .Restrict depending on your requirements
            });

            // Configure Album entity
            builder.Entity<Album>(entity =>
            {
                entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entity.Property(a => a.ReleaseDate).IsRequired();
                entity.HasOne(a => a.Artist)
                      .WithMany(a => a.Albums)
                      .HasForeignKey(a => a.ArtistId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });


            builder.Entity<Friendship>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequesterId).IsRequired();
                entity.Property(e => e.FriendId).IsRequired();
                entity.Property(e => e.IsAccepted).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.RequestDate).IsRequired();

                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Friend)
                      .WithMany()
                      .HasForeignKey(e => e.FriendId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Message>(entity =>
            {
                entity.Property(m => m.Content).IsRequired();
                entity.Property(m => m.SentAt).IsRequired();

                entity.HasOne(m => m.Sender)
                      .WithMany()
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                      .WithMany()
                      .HasForeignKey(m => m.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
