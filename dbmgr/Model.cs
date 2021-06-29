using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dbmgr
{
    public class Subject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required, Column(TypeName = "varchar(500)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? NameCN { get; set; }
        [Required, Column(TypeName = "varchar(10)")]
        public string Type { get; set; }
        public int? Rank { get; set; }
        public DateTime? Date { get; set; }
        public int Votenum { get; set; }
        public int Favnum { get; set; }

        public List<Tag> Tags;
        public CustomRank ScientificRank;
    }

    [Index(nameof(SubjectId), nameof(Content), IsUnique = true)]
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        public int SubjectId { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string Content { get; set; }

        public int TagCount { get; set; }
        public int UserCount { get; set; }
        public double Confidence { get; set; }

        public Subject Subject;
    }

    public class CustomRank
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubjectId { get; set; }
        public int SciRank { get; set; }
        public DateTime Date { get; set; }

        public Subject Subject;
    }

    public class BangumiContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CustomRank> CustomRanks { get; set; }

        //public BangumiContext(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 127.0.0.1; Port = 5432; Database = bangumi; User Id = postgres; Password = password; Include Error Detail = True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().HasOne(t => t.Subject).WithMany(sub => sub.Tags).HasForeignKey(t => t.SubjectId);
            modelBuilder.Entity<CustomRank>().HasOne(cr => cr.Subject).WithOne(sub => sub.ScientificRank).HasForeignKey<CustomRank>(cr => cr.SubjectId);
        }
    }
}
