using Microsoft.EntityFrameworkCore;
using TwitterApp.Models;

namespace TwitterApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<TweetHashtag> TweetHashtags { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User - Follows (Self-referencing many-to-many)
        modelBuilder.Entity<UserFollow>()
            .HasKey(uf => new { uf.FollowerId, uf.FolloweeId });

        modelBuilder.Entity<UserFollow>()
            .HasOne(uf => uf.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserFollow>()
            .HasOne(uf => uf.Followee)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tweet - User (One-to-Many)
        modelBuilder.Entity<Tweet>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Like - Composite Key & Relationships
        modelBuilder.Entity<Like>()
            .HasKey(l => new { l.UserId, l.TweetId });

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Tweet)
            .WithMany(t => t.Likes)
            .HasForeignKey(l => l.TweetId)
            .OnDelete(DeleteBehavior.Cascade);

        // TweetHashtag - Composite Key & Relationships
        modelBuilder.Entity<TweetHashtag>()
            .HasKey(th => new { th.TweetId, th.HashtagId });

        modelBuilder.Entity<TweetHashtag>()
            .HasOne(th => th.Tweet)
            .WithMany(t => t.TweetHashtags)
            .HasForeignKey(th => th.TweetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TweetHashtag>()
            .HasOne(th => th.Hashtag)
            .WithMany(h => h.TweetHashtags)
            .HasForeignKey(th => th.HashtagId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }


    }
}
