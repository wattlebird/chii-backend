﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using chii.Models;
namespace chii.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class Tags : ControllerBase
    {
        private readonly BangumiContext _context;

        public Tags(BangumiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BriefTag>>> GetAllTags() {
            var tags = await _context.Tags
                .GroupBy(tag => tag.Content, (key, tags) => new BriefTag
                    {
                        Tag = key,
                        Coverage = tags.Count(),
                        Confidence = tags.Average(t => t.Confidence)
                    })
                .Where(x => x.Coverage > 20)
                .OrderByDescending(x => x.Coverage).ThenByDescending(x => x.Confidence)
                .ToListAsync();
            return tags;
        }

        [HttpPost("search")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<ClientSubject>>> SearchTags(SearchField searchField)
        {
            List<string> tags = new List<string>();
            int minVoters = 0, minFavers = 0;
            if (searchField.tags?.Count() > 0)
            {
                tags = searchField.tags;
            }
            if (searchField.minVoters != null) minVoters = searchField.minVoters.Value;
            if (searchField.minFavs != null) minFavers = searchField.minFavs.Value;

            var candidates = await _context.Tags
                .Where(t => tags.Contains(t.Content) && t.Confidence > 1e-3 && t.Subject.Favnum > minFavers && t.Subject.Votenum > minVoters)
                .ToListAsync();
            var aggregated = candidates.GroupBy(t => t.Content, t => t.SubjectId).ToList();
            if (aggregated.Count() < tags.Count()) return NotFound();
            int idx = 0;
            List<int> subjectIds = new List<int>();
            aggregated.ForEach(itm =>
            {
                if (idx == 0)
                {
                    subjectIds = itm.ToList();
                }
                else
                {
                    subjectIds = subjectIds.Intersect(itm.ToList()).ToList();
                }
                idx++;
            });
            if (subjectIds.Count() == 0) return NotFound();
            var rtn = await _context.Subjects.Where(sub => subjectIds.Contains(sub.Id)).Include(sub => sub.ScientificRank).Select(subject => new ClientSubject
            {
                Id = subject.Id,
                Name = subject.Name,
                NameCN = subject.NameCN,
                Type = subject.Type,
                SciRank = subject.ScientificRank.SciRank,
                Rank = subject.Rank,
                Date = subject.Date,
            }).ToListAsync();
            return rtn;
        }

        [HttpPost("related")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<BriefTag>>> GetRelatedTags(RelatedTagField   )
        {
            List<string> tags = new List<string>();
            if (relatedTagField.tags?.Count() > 0)
            {
                tags = relatedTagField.tags;
            }
            if (tags.Count() == 0)
            {
                return new List<BriefTag>();
            }
            var candidates = await _context.Tags
                .Where(t => tags.Contains(t.Content) && t.Confidence > 1e-3)
                .Select(t => t.SubjectId)
                .ToListAsync();
            Dictionary<int, int> cnt = new Dictionary<int, int>();
            candidates.ForEach(subject =>
            {
                if (cnt.ContainsKey(subject)) cnt[subject] += 1;
                else cnt[subject] = 1;
            });
            List<int> relatedSubjects = new List<int>();
            foreach(var kv in cnt)
            {
                if (kv.Value == tags.Count) relatedSubjects.Add(kv.Key);
            }
            if (relatedSubjects.Count == 0) return new List<BriefTag>();
            var rtn = await _context.Tags
                .Where(t => relatedSubjects.Contains(t.SubjectId) && t.Confidence > 1e-3)
                .GroupBy(tag => tag.Content, (key, tags) => new BriefTag
                {
                    Tag = key,
                    Coverage = tags.Count(),
                    Confidence = tags.Average(t => t.Confidence)
                })
                .OrderByDescending(x => x.Coverage).ThenByDescending(x => x.Confidence)
                .ToListAsync();
            return rtn;
        }

    }
}
