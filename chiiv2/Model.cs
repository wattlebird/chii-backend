using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace chiiv2
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Subject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "name_cn")]
        public string? NameCN { get; set; }
        [JsonProperty(PropertyName = "infobox")]
        public string? Infobox { get; set; }
        [JsonProperty(PropertyName = "platform")]
        public int Platform { get; set; }
        [JsonProperty(PropertyName = "summary")]
        public string? Summary { get; set; }
        [JsonProperty(PropertyName = "rank")]
        public int? Rank { get; set; }
        [JsonProperty(PropertyName = "nsfw")]
        public bool NSFW { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string? Type { get; set; }
        [JsonProperty(PropertyName = "fav_count")]
        public int FavCount { get; set; }
        [JsonProperty(PropertyName = "rate_count")]
        public int RateCount { get; set; }
        [JsonProperty(PropertyName = "collect_count")]
        public int CollectCount { get; set; }
        [JsonProperty(PropertyName = "do_count")]
        public int DoCount { get; set; }
        [JsonProperty(PropertyName = "dropped_count")]
        public int DroppedCount { get; set; }
        [JsonProperty(PropertyName = "on_hold_count")]
        public int OnHoldCount { get; set; }
        [JsonProperty(PropertyName = "wish_count")]
        public int WishCount { get; set; }

        public List<Tag> Tags;
        public List<SubjectEntity> SubjectEntities;
        public CustomRank ScientificRank;
    }

    [Index(nameof(SubjectId), nameof(NormalizedContent), IsUnique = true)]
    [JsonObject(MemberSerialization.OptIn)]
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "iid")]
        public int SubjectId { get; set; }
        [JsonProperty(PropertyName = "tags")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "tags_normalized")]
        public string NormalizedContent { get; set; }
        [JsonProperty(PropertyName = "cnt")]
        public int UserCount { get; set; }
        [JsonProperty(PropertyName = "confidence")]
        public double Confidence { get; set; }

        public Subject Subject;
    }

    [Index(nameof(NormalizedAlias))]
    [JsonObject(MemberSerialization.OptIn)]
    public class SubjectEntity
    {
        [Key]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int SubjectId { get; set; }
        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }
        [JsonProperty(PropertyName = "alias_normalized")]
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
