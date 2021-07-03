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
        public async Task<ActionResult<IEnumerable<BriefTag>>> GetAllTags([FromQuery] string type="anime") {
            var tags = await _context.Tags
                .Where(tag => tag.Subject.Type == type)
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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClientSubject>>> SearchTags([FromQuery] string tags, [FromQuery] string type = "anime")
        {
            List<string> vs = tags.Trim().Split().ToList();
            var candidates = await _context.Tags.Where(t => vs.Contains(t.Content) && t.Subject.Type == type && t.Confidence > 1e-3).ToListAsync();
            var aggregated = candidates.GroupBy(t => t.Content, t => t.SubjectId).ToList();
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
            var rtn = await _context.Subjects.Where(sub => subjectIds.Contains(sub.Id)).Select(subject => new ClientSubject
            {
                Id = subject.Id,
                Name = subject.Name,
                NameCN = subject.NameCN,
                Type = subject.Type,
                Rank = subject.Rank,
                Date = subject.Date,
                Votenum = subject.Votenum,
                Favnum = subject.Favnum
            }).ToListAsync();
            return rtn;
        }

        [HttpPost("related")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<BriefTag>>> GetRelatedTags(List<string> tags, [FromQuery] string type = "anime")
        {
            var candidates = await _context.Tags
                .Where(t => tags.Contains(t.Content) && t.Subject.Type == type && t.Confidence > 1e-3)
                .Select(t => t.SubjectId)
                .Distinct()
                .ToListAsync();
            var rtn = await _context.Tags
                .Where(t => candidates.Contains(t.SubjectId) && t.Subject.Type == type && t.Confidence > 1e-3)
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