using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using chii.Models;
namespace chii.Controllers
{
    [Route("api/misc")]
    [ApiController]
    public class Miscellaneous: ControllerBase
    {
        private readonly BangumiContext _context;

        public Miscellaneous(BangumiContext context)
        {
            _context = context;
        }

        [HttpGet("lastdate")]
        public async Task<DateTime> GetLastUpdateTime()
        {
            var rank = await _context.CustomRanks.FirstAsync();
            return rank.Date;
        }
    }
}
