using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace chiiv2
{
    public class Database
    {
        private readonly BangumiContext _context;
        public Database(BangumiContext context)
        {
            _context = context;
        }

        [FunctionName("syncrank")]
        [StorageAccount("IkelyConnectionString")]
        public void SyncRank([BlobTrigger("database/{name}")]Stream stream, string name, ILogger log)
        {
            if (name.StartsWith("customrank") && name.EndsWith("csv"))
            {
                log.LogInformation($"Function SyncRank processing file {name}...");
                name = name.Split('.')[0];
                string[] vs = name.Split('_');
                DateTime date = new DateTime(Int32.Parse(vs[1]), Int32.Parse(vs[2]), Int32.Parse(vs[3]));
                Timestamp ts = _context.Timestamps.First();
                if (ts.Date == date)
                {
                    log.LogInformation($"Customrank on {date.ToString()} already exists, exit.");
                    return;
                }

                List<CustomRank> ranks = new List<CustomRank>();
                using (var reader = new StreamReader(stream))
                {
                    reader.ReadLine();
                    while(reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine().Trim();
                        string[] parts = line.Split(new[] { ',' });
                        CustomRank rank = new CustomRank
                        {
                            SubjectId = Int32.Parse(parts[0]),
                            SciRank = Int32.Parse(parts[1]),
                        };
                        ranks.Add(rank);
                    }
                }
                log.LogInformation($"Read {ranks.Count()} customrank records.");

                int count = _context.CustomRanks.Count();
                if (count > 0)
                {
                    _context.CustomRanks.RemoveRange(_context.CustomRanks);
                    _context.SaveChanges();
                }
                _context.CustomRanks.AddRange(ranks);
                _context.SaveChanges();
                log.LogInformation($"Customrank updated.");


                _context.Timestamps.RemoveRange(_context.Timestamps);
                _context.Timestamps.Add(new Timestamp { Date = date });
                _context.SaveChanges();
                log.LogInformation($"Timestamp updated.");
            }
        }
    }
}
