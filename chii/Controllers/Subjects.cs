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

        // GET: api/Subjects
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ClientSubject>>> GetSubjects([FromQuery] string type = "anime", [FromQuery] int from = 0, [FromQuery] int step = 20)
        {
            var subjects = await _context.Subjects.Where(x => x.Type == type).OrderBy(x => x.Rank).Skip(from).Take(step)
                .Include(sub => sub.Tags.OrderByDescending(t => t.Confidence).Take(5)).ToListAsync();
            if (subjects.Count == 0)
            {
                return NotFound();
            }
            var rtn = subjects.Select(sub => new ClientSubject
            {
                Id = sub.Id,
                Name = sub.Name,
                NameCN = sub.NameCN,
                Type = sub.Type,
                Rank = sub.Rank,
                Date = sub.Date,
                Votenum = sub.Votenum,
                Favnum = sub.Favnum,
                Tags = sub.Tags.Select(tag => new ClientTag
                {
                    Tag = tag.Content,
                    TagCount = tag.TagCount,
                    UserCount = tag.UserCount,
                    Confidence = tag.Confidence
                }).ToList()
            }).ToList();
            return rtn;
        }

        [HttpGet("ranked")]
        public async Task<ActionResult<IEnumerable<ClientSubject>>> GetRankedSubjects([FromQuery] string type = "anime", [FromQuery] int from = 0, [FromQuery] int step = 20)
        {
            var subjects = await _context.Subjects.Where(x => x.Type == type && x.Rank != null).OrderBy(x => x.Rank).Skip(from).Take(step)
                .Include(sub => sub.Tags.OrderByDescending(t => t.Confidence).Take(5))
                .Include(sub => sub.ScientificRank).ToListAsync();
            if (subjects.Count == 0)
            {
                return NotFound();
            }
            var rtn = subjects.Select(sub => new ClientSubject
            {
                Id = sub.Id,
                Name = sub.Name,
                NameCN = sub.NameCN,
                Type = sub.Type,
                Rank = sub.Rank,
                SciRank = sub.ScientificRank.SciRank,
                Date = sub.Date,
                Votenum = sub.Votenum,
                Favnum = sub.Favnum,
                Tags = sub.Tags.Select(tag => new ClientTag
                {
                    Tag = tag.Content,
                    TagCount = tag.TagCount,
                    UserCount = tag.UserCount,
                    Confidence = tag.Confidence
                }).ToList()
            }).ToList();
            return rtn;
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientSubject>> GetSubject(int id)
        {
            var subject = await _context.Subjects.Where(x => x.Id == id)
                .Include(sub => sub.Tags).FirstAsync();
            if (subject == null)
            {
                return NotFound();
            }
            var rtn = new ClientSubject
            {
                Id = subject.Id,
                Name = subject.Name,
                NameCN = subject.NameCN,
                Type = subject.Type,
                Rank = subject.Rank,
                Date = subject.Date,
                Votenum = subject.Votenum,
                Favnum = subject.Favnum,
                Tags = subject.Tags.Select(tag => new ClientTag
                {
                    Tag = tag.Content,
                    TagCount = tag.TagCount,
                    UserCount = tag.UserCount,
                    Confidence = tag.Confidence
                }).ToList()
            };
            return rtn;
        }

        [HttpGet("count")]
        public async Task<int> GetSubjectCount([FromQuery] string type = "anime", [FromQuery] bool ranked = true)
        {
            var cnt = await _context.Subjects.Where(sub => sub.Type == type && (sub.Rank != null || !ranked)).CountAsync();
            return cnt;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClientSubject>>> SearchSubject([FromQuery] string keyword, [FromQuery] string type = "anime")
        {
            var subs = await _context.Subjects
                .Where(sub => (sub.Name.Contains(keyword) || sub.NameCN.Contains(keyword)) && sub.Type == type)
                .OrderBy(sub => sub.Rank)
                .Include(sub => sub.Tags.OrderByDescending(t => t.Confidence).Take(5))
                .ToListAsync();
            if (subs.Count == 0)
            {
                return NotFound();
            }

            var rtn = subs.Select(sub => new ClientSubject
            {
                Id = sub.Id,
                Name = sub.Name,
                NameCN = sub.NameCN,
                Type = sub.Type,
                Rank = sub.Rank,
                Date = sub.Date,
                Votenum = sub.Votenum,
                Favnum = sub.Favnum,
                Tags = sub.Tags.Select(tag => new ClientTag
                {
                    Tag = tag.Content,
                    TagCount = tag.TagCount,
                    UserCount = tag.UserCount,
                    Confidence = tag.Confidence
                }).ToList()
            }).ToList();
            return rtn;
        }
        
    }
}
