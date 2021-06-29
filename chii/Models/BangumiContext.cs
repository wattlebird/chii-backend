using System;
using Microsoft.EntityFrameworkCore;

namespace chii.Models
{
    public class BangumiContext : DbContext
    {
        public BangumiContext(DbContextOptions<BangumiContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().HasOne(t => t.Subject).WithMany(sub => sub.Tags).HasForeignKey(t => t.SubjectId);
            modelBuilder.Entity<CustomRank>().HasOne(cr => cr.Subject).WithOne(sub => sub.ScientificRank).HasForeignKey<CustomRank>(cr => cr.SubjectId);
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CustomRank> CustomRanks { get; set; }
    }
}
