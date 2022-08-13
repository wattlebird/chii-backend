using System;
using System.Collections.Generic;

namespace chii.Models
{
    public class SubjectResponse
    {
        public int Id { get; set; }
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
        public double? Score { get; set; }

        public List<TagResponse> Tags;
        public CustomRankResponse ScientificRank;
    }

    public class TagResponse
    {
        public string Content { get; set; }
        public int UserCount { get; set; }
        public double Confidence { get; set; }
    }

    public class CustomRankResponse
    {
        public int SciRank { get; set; }
    }
}
