using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using chii.Models;

namespace chii.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class Subjects : ControllerBase
    {
        private readonly BangumiContext _context;

        public Subjects(BangumiContext context)
        {
            _context = context;
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectResponse>> GetSubject(int id)
        {
            Subject subject = await _context.Subjects.Where(x => x.Id == id)
                .Include(sub => sub.ScientificRank)
                .Include(sub => sub.Tags).FirstOrDefaultAsync();
            if (subject == null)
            {
                return NotFound();
            }
            SubjectResponse rtn = new SubjectResponse
            {
                Id = subject.Id,
                Name = subject.Name,
                NameCN = subject.NameCN,
                Infobox = subject.Infobox,
                Platform = subject.Platform,
                Summary = subject.Summary,
                Rank = subject.Rank,
                NSFW = subject.NSFW,
                Type = subject.Type,
                FavCount = subject.FavCount,
                RateCount = subject.RateCount,
                CollectCount = subject.CollectCount,
                DoCount = subject.DoCount,
                DroppedCount = subject.DroppedCount,
                OnHoldCount = subject.OnHoldCount,
                WishCount = subject.WishCount,
                Tags = subject?.Tags.Select(tag => new TagResponse
                {
                    Content = tag.Content,
                    Confidence = tag.Confidence,
                    UserCount = tag.UserCount
                }).ToList(),
                ScientificRank = subject.ScientificRank is null ? null : new CustomRankResponse
                {
                    SciRank = subject.ScientificRank.SciRank
                },
            };
            return rtn;
        }

        [HttpGet("ranking")]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetRankedSubjects([FromQuery] string type = "anime")
        {
            List<Subject> subjects = await _context.Subjects.Where(x => x.Type == type && x.Rank != null)
                .Include(sub => sub.ScientificRank)
                .OrderBy(x => x.Rank).ToListAsync();
            if (subjects.Count == 0)
            {
                return NotFound();
            }
            return subjects.Select(sub => new SubjectResponse
            {
                Id = sub.Id,
                Name = sub.Name,
                NameCN = sub.NameCN,
                Infobox = sub.Infobox,
                Platform = sub.Platform,
                Summary = sub.Summary,
                Rank = sub.Rank,
                NSFW = sub.NSFW,
                Type = sub.Type,
                FavCount = sub.FavCount,
                RateCount = sub.RateCount,
                CollectCount = sub.CollectCount,
                DoCount = sub.DoCount,
                DroppedCount = sub.DroppedCount,
                OnHoldCount = sub.OnHoldCount,
                WishCount = sub.WishCount,
                ScientificRank = sub.ScientificRank is null ? null : new CustomRankResponse
                {
                    SciRank = sub.ScientificRank.SciRank
                },
            }).ToList();
        }
    }
}
