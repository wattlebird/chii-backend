using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace chiiv2
{
    public class Subject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? NameCN { get; set; }
        public string? Infobox { get; set; }
        public int Platform { get; set; }
        public string? Summary { get; set; }
        public int? Rank { get; set; }
        public bool NSFW { get; set; }
        public string? Type { get; set; }
        public int FavCount { get; set; }
        public int RateCount { get; set; }
        public int CollectCount { get; set; }
        public int DoCount { get; set; }
        public int DroppedCount { get; set; }
        public int OnHoldCount { get; set; }
        public int WishCount { get; set; }

        public List<Tag> Tags;
        public List<SubjectEntity> SubjectEntities;
        public CustomRank ScientificRank;
    }

    [Index(nameof(SubjectId), nameof(NormalizedContent), IsUnique = true)]
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string Content { get; set; }
        public string NormalizedContent { get; set; }
        public int UserCount { get; set; }
        public double Confidence { get; set; }

        public Subject Subject;
    }

    [Index(nameof(NormalizedAlias))]
    public class SubjectEntity
    {
        [Key]
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string Alias { get; set; }
        public string NormalizedAlias { get; set; }

        public Subject Subject;
    }

    public class CustomRank
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubjectId { get; set; }
        public int SciRank { get; set; }

        public Subject Subject;
    }

    public class Timestamp
    {
        [Key]
        public DateTime Date { get; set; }
    }

    public class BangumiContext : DbContext
    {

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<SubjectEntity> SubjectEntities { get; set; }
        public DbSet<CustomRank> CustomRanks { get; set; }
        public DbSet<Timestamp> Timestamps { get; set; }

        public BangumiContext(DbContextOptions<BangumiContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().HasOne(t => t.Subject).WithMany(sub => sub.Tags).HasForeignKey(t => t.SubjectId);
            modelBuilder.Entity<CustomRank>().HasOne(cr => cr.Subject).WithOne(sub => sub.ScientificRank);
            modelBuilder.Entity<SubjectEntity>().HasOne(s => s.Subject).WithMany(sub => sub.SubjectEntities).HasForeignKey(s => s.SubjectId);
        }
    }
}
