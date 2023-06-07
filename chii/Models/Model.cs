using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace chii.Models
{
    public class NullableFloatToIntConverter : JsonConverter<int?>
    {
        public override int? ReadJson(JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (!float.TryParse(reader.Value.ToString(), out var floatValue))
                return null;

            return (int)floatValue;
        }
        
        public override void WriteJson(JsonWriter writer, int? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                writer.WriteValue(value.ToString());
            }
            else
            {
                writer.WriteNull();
            }
            
        }
    }
    
    [JsonObject(MemberSerialization.OptIn)]
    public class Subject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [Required, Column(TypeName = "varchar(80)")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(80)")]
        [JsonProperty(PropertyName = "name_cn")]
        public string? NameCN { get; set; }
        [Column(TypeName = "json")]
        [JsonProperty(PropertyName = "infobox")]
        public string? Infobox { get; set; }
        [JsonProperty(PropertyName = "platform")]
        public int Platform { get; set; }
        [Column(TypeName = "text")]
        [JsonProperty(PropertyName = "summary")]
        public string? Summary { get; set; }
        [JsonProperty(PropertyName = "rank")]
        [JsonConverter(typeof(NullableFloatToIntConverter))]
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

    [Index(nameof(SubjectId), nameof(NormalizedContent))]
    [JsonObject(MemberSerialization.OptIn)]
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "iid")]
        public int SubjectId { get; set; }
        [Column(TypeName = "text")]
        [JsonProperty(PropertyName = "tags")]
        public string Content { get; set; }
        [Column(TypeName = "text")]
        [JsonProperty(PropertyName = "tags_normalized")]
        public string NormalizedContent { get; set; }
        [JsonProperty(PropertyName = "tag_cnt")]
        public int UserCount { get; set; }
        [JsonProperty(PropertyName = "tagnorm_cnt")]
        public int NormUserCount { get; set; }
        [JsonProperty(PropertyName = "confidence")]
        public double Confidence { get; set; }

        public Subject Subject;
    }

    [Index(nameof(SubjectId), nameof(NormalizedAlias))]
    [JsonObject(MemberSerialization.OptIn)]
    public class SubjectEntity
    {
        [Key]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int SubjectId { get; set; }
        [Column(TypeName = "text")]
        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }
        [Column(TypeName = "text")]
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
}
