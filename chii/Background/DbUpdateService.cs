using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using chii.Models;
using Microsoft.Extensions.DependencyInjection;

namespace chii.Background
{
    public class DbUpdateService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IConfiguration _config;
        private readonly ILogger<DbUpdateService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DbUpdateService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration, ILogger<DbUpdateService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _config = configuration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Set up db update cron job.");

            _timer = new Timer(UpdateDatabase, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void UpdateDatabase(object state)
        {
            //https://stackoverflow.com/questions/51572637/access-dbcontext-service-from-background-task
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<BangumiContext>();
                bool isEmptyDb = !_db.CustomRanks.Any();
                DateTime dbDate = new DateTime(), lastDate = new DateTime();
                if (!isEmptyDb)
                {
                    dbDate = _db.CustomRanks.First().Date;
                    lastDate = await GetLastFileDate("bangumi-publish", "ranking", "customrank_");
                }
                if (isEmptyDb || dbDate != lastDate)
                {
                    _logger.LogInformation($"Db date {dbDate.ToString()} doesn't match fileshare date {lastDate.ToString()}, will update.");
                    await UpdateSubjectDb(_db);
                    await UpdateTagDb(_db);
                    await UpdateRankDb(_db);
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Tear down db update cron job.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task<DateTime> GetLastFileDate(string fileshare, string dir, string prefix)
        {
            _logger.LogInformation($"Getting latest date of {dir}.");
            string connectionString = _config["AZURE_FILESHARE_CONNECTIONSTRING"];
            Regex dateRegex = new Regex(@"(\d+)_(\d+)_(\d+)", RegexOptions.Compiled);
            // Download subject and ranking file
            ShareClient share = new ShareClient(connectionString, fileshare);
            ShareDirectoryClient directory = share.GetDirectoryClient(dir);
            var items = directory.GetFilesAndDirectoriesAsync(prefix);
            // Get the last date
            DateTime date = new DateTime(2021, 1, 1);
            await foreach (var item in items)
            {
                var match = dateRegex.Match(item.Name);
                if (match.Success)
                {
                    DateTime newdate = new DateTime(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value));
                    if (newdate.CompareTo(date) > 0)
                    {
                        date = newdate;
                    }
                }
            }
            _logger.LogInformation($"Latest date of {dir} is {date.ToString()}");
            return date;
        }

        private async Task DownloadFile(string fileshare, string dir, string filename)
        {
            _logger.LogInformation($"Download {filename} to working directory.");
            string connectionString = _config["AZURE_FILESHARE_CONNECTIONSTRING"];
            ShareClient share = new ShareClient(connectionString, fileshare);
            ShareDirectoryClient directory = share.GetDirectoryClient(dir);
            ShareFileClient file = directory.GetFileClient(filename);
            ShareFileDownloadInfo dwn = await file.DownloadAsync();
            if (!File.Exists($"./{filename}"))
            {
                using (FileStream stream = File.OpenWrite($"./{filename}"))
                {
                    await dwn.Content.CopyToAsync(stream);
                }
            }
            else
            {
                _logger.LogInformation($"File {filename} already exists.");
            }
        }

        private async Task UpdateSubjectDb(BangumiContext db)
        {
            const string shareName = @"bangumi-publish";
            DateTime lastDate = await GetLastFileDate(shareName, "subject", "subject_");
            string lastSubjectFile = "subject_" + lastDate.ToString("yyyy_MM_dd") + ".tsv";
            await DownloadFile(shareName, "subject", lastSubjectFile);

            StreamReader fSubject = new StreamReader($"./{lastSubjectFile}");

            _logger.LogInformation("Reading all subjects...");
            string line;
            List<Subject> subjects = new List<Subject>();
            while ((line = fSubject.ReadLine()) != null)
            {
                var items = line.Split('\t');
                int votenum = Convert.ToInt32(items[6]);
                int favnum = items[7].Split(';').Select(i => Convert.ToInt32(i)).Sum();
                DateTime dateTime;
                DateTime.TryParse(items[5], out dateTime);
                subjects.Add(new Subject
                {
                    Id = Convert.ToInt32(items[0]),
                    Name = items[1],
                    NameCN = String.IsNullOrEmpty(items[2]) ? null : items[2],
                    Type = items[3],
                    Rank = String.IsNullOrEmpty(items[4]) ? null : Convert.ToInt32(items[4]),
                    Date = dateTime,
                    Votenum = votenum,
                    Favnum = favnum
                });
            }

            var s = db.Subjects.Count();
            if (s != 0)
            {
                db.Subjects.RemoveRange(db.Subjects);
            }
            db.AddRange(subjects);
            db.SaveChanges();
            _logger.LogInformation("Subjects archived successfully.");
        }

        private async Task UpdateTagDb(BangumiContext db)
        {
            const string shareName = @"bangumi-publish";
            DateTime lastDate = await GetLastFileDate(shareName, "tags", "customtags_");
            string lastTagFile = "customtags_" + lastDate.ToString("yyyy_MM_dd") + ".tsv";
            await DownloadFile(shareName, "tags", lastTagFile);

            StreamReader fTag = new StreamReader($"./{lastTagFile}");

            _logger.LogInformation("Reading all tags...");
            string line = fTag.ReadLine(); // skip header
            List<Tag> tags = new List<Tag>();
            int cnt = 0;
            var s = db.Tags.Count();
            if (s != 0)
            {
                db.Tags.RemoveRange(db.Tags);
            }

            while ((line = fTag.ReadLine()) != null)
            {
                var items = line.Split('\t');
                if (String.IsNullOrWhiteSpace(items[1]) || items[1].Length > 256) continue;
                tags.Add(new Tag
                {
                    SubjectId = Convert.ToInt32(items[0]),
                    Content = Regex.Replace(items[1], @"\s+", ""),
                    TagCount = Convert.ToInt32(items[2]),
                    UserCount = Convert.ToInt32(items[3]),
                    Confidence = Convert.ToDouble(items[4])
                });
                cnt++;

                if (cnt == 100000)
                {
                    cnt = 0;
                    db.Tags.AddRange(tags);
                    db.SaveChanges();
                    tags.Clear();
                }
            }

            if (cnt != 0)
            {
                db.Tags.AddRange(tags);
                db.SaveChanges();
            }
            _logger.LogInformation("Tags archived successfully.");
        }

        private async Task UpdateRankDb(BangumiContext db)
        {
            const string shareName = @"bangumi-publish";
            DateTime lastDate = await GetLastFileDate(shareName, "ranking", "customrank_");
            string lastRankFile = "customrank_" + lastDate.ToString("yyyy_MM_dd") + ".csv";
            await DownloadFile(shareName, "ranking", lastRankFile);

            StreamReader fRank = new StreamReader($"./{lastRankFile}");

            _logger.LogInformation("Reading all ranks...");
            string line = fRank.ReadLine(); // skip header
            List<CustomRank> ranks = new List<CustomRank>();
            while ((line = fRank.ReadLine()) != null)
            {
                var items = line.Split(',');
                ranks.Add(new CustomRank
                {
                    SubjectId = Convert.ToInt32(items[0]),
                    SciRank = Convert.ToInt32(items[1]),
                    Date = lastDate
                });
            }

            var s = db.CustomRanks.Count();
            if (s != 0)
            {
                db.CustomRanks.RemoveRange(db.CustomRanks);
            }
            db.CustomRanks.AddRange(ranks);
            db.SaveChanges();
            _logger.LogInformation("Ranking archived successfully.");
        }
    }
}
