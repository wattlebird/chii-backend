using System;
using System.Collections.Generic;
namespace chii.Models
{
    public class ClientSubject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? NameCN { get; set; }
        public string Type { get; set; }
        public int? Rank { get; set; }
        public int? SciRank { get; set; }
        public DateTime? Date { get; set; }
        public int Votenum { get; set; }
        public int Favnum { get; set; }
        public List<ClientTag> Tags { get; set; }
    }

    public class ClientTag
    {
        public string Tag { get; set; }
        public int TagCount { get; set; }
        public int UserCount { get; set; }
        public double Confidence { get; set; }
    }

    public class BriefTag
    {
        public string Tag { get; set; }
        public int Coverage { get; set; }
        public double Confidence { get; set; }
    }

    public class SearchField
    {
        public List<string>? tags { get; set; }
        public int? minVoters { get; set; }
        public int? minFavs { get; set; }
    }

    public class RelatedTagField
    {
        public List<string>? tags { get; set; }
    }
}
