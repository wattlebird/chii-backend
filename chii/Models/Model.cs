using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace chii.Models
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
}
