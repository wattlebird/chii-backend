using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using chii.Models;
namespace chii.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class Search : ControllerBase
    {
        private readonly BangumiContext _context;

        public Search(BangumiContext context)
        {
            _context = context;
        }

        [HttpGet("autocomplete")]
        public async Task<ActionResult<IEnumerable<string>>> AutoComplete([FromQuery] string q)
        {
            q = q.Trim().ToLowerInvariant();
            q = Regex.Replace(q, @"\W", "");
            q = ChineseConverter.Convert(q, ChineseConversionDirection.TraditionalToSimplified);
            if (String.IsNullOrEmpty(q)) return null;

            var tagCandidates = _context.Tags.Where(tag => tag.NormalizedContent.StartsWith(q)).GroupBy(tag => new { tag.Content, tag.NormalizedContent }, (key, tags) => new
            {
                Content = key.Content,
                NormalizedContent = key.NormalizedContent,
                Hit = tags.Sum(t => t.UserCount)
            }); 
            var selectedTags = tagCandidates.AsEnumerable().GroupBy(itm => itm.NormalizedContent, (key, grp) => new
            {
                Content = grp.MaxBy(t => t.Hit).Content,
                Hit = grp.Max(t => t.Hit),
            }).OrderByDescending(t => t.Hit);
            if (selectedTags.Count() > 10)
            {
                return selectedTags.Take(10).Select(itm => itm.Content).ToArray<string>();
            }
            else
            {
                return selectedTags.Select(item => item.Content).ToArray<string>();
            }
        }

        [HttpGet("related")]
        public async Task<ActionResult<IEnumerable<TagResponse>>> Related([FromQuery] string q)
        {
            string[] vq = q.Split(' ');
            vq = vq.Select(eq => ChineseConverter.Convert(Regex.Replace(HttpUtility.UrlDecode(eq).Trim().ToLowerInvariant(), @"\W", ""), ChineseConversionDirection.TraditionalToSimplified))
                .Where(eq => !String.IsNullOrEmpty(eq)).ToArray<string>();
            var relatedSubjects = _context.Tags
                .Where(t => vq.Contains(t.NormalizedContent) && t.Confidence > 1e-3)
                .GroupBy(t => new { t.SubjectId, t.NormalizedContent }, (key, tags) => new { SubjectId = key.SubjectId, Tag = key.NormalizedContent, Confidence = tags.Max(x => x.Confidence) })
                .GroupBy(t => t.SubjectId, (key, tags) => new { SubjectId = key, Hits = tags.Count() })
                .Where(item => item.Hits == vq.Length);
            var relatedTags = _context.Tags.Join(relatedSubjects, tags => tags.SubjectId, subject => subject.SubjectId,
                (tags, subject) => tags).Where(tag => tag.Confidence > 1e-2 && tag.UserCount > 2 && !vq.Contains(tag.NormalizedContent))
                .GroupBy(tag => tag.Content, (key, tags) => new TagResponse { Content = key, UserCount = tags.Sum(tag => tag.UserCount), Confidence = tags.Average(tag => tag.Confidence) })
                .OrderByDescending(x => x.UserCount).ThenByDescending(x => x.Confidence);
            return await relatedTags.ToListAsync();

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> TagSearch([FromQuery] string q)
        {
            string[] vq = q.Split(' ');
            vq = vq.Select(eq => ChineseConverter.Convert(Regex.Replace(HttpUtility.UrlDecode(eq).Trim().ToLowerInvariant(), @"\W", ""), ChineseConversionDirection.TraditionalToSimplified))
                .Where(eq => !String.IsNullOrEmpty(eq)).ToArray<string>();
            var relatedSubjectIds = _context.Tags
                .Where(t => vq.Contains(t.NormalizedContent) && t.Confidence > 1e-3)
                .GroupBy(t => new { t.SubjectId, t.NormalizedContent }, (key, tags) => new { SubjectId = key.SubjectId, Tag = key.NormalizedContent, Confidence = tags.Max(x => x.Confidence) })
                .GroupBy(t => t.SubjectId, (key, tags) => new { SubjectId = key, Hits = tags.Count(), Confidence = tags.Average(t => t.Confidence) })
                .Where(item => item.Hits == vq.Length);
            var relatedSubjects = _context.Subjects.Include(sub => sub.ScientificRank)
                .Join(relatedSubjectIds, left => left.Id, right => right.SubjectId, (left, right) => new SubjectResponse
                {
                    Id = left.Id,
                    Name = left.Name,
                    NameCN = left.NameCN,
                    Infobox = left.Infobox,
                    Platform = left.Platform,
                    Summary = left.Summary,
                    Rank = left.Rank,
                    NSFW = left.NSFW,
                    Type = left.Type,
                    FavCount = left.FavCount,
                    RateCount = left.RateCount,
                    CollectCount = left.CollectCount,
                    DoCount = left.DoCount,
                    DroppedCount = left.DroppedCount,
                    OnHoldCount = left.OnHoldCount,
                    WishCount = left.WishCount,
                    Score = right.Confidence,
                    ScientificRank = left.ScientificRank == null ? null : new CustomRankResponse
                    {
                        SciRank = left.ScientificRank.SciRank
                    },
                }).OrderByDescending(itm => itm.Score);
            return await relatedSubjects.ToArrayAsync();

        }
    }
}

