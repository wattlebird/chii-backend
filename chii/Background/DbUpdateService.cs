using Azure.Storage.Blobs;
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
using Newtonsoft.Json;

namespace chii.Background
{
    internal interface ISyncService
    {
        Task Sync();
    }

    internal class SyncService: ISyncService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly BangumiContext _db;

        public SyncService(BangumiContext bangumiContext, IConfiguration configuration, ILogger<SyncService> logger)
        {
            _logger = logger;
            _db = bangumiContext;
            _config = configuration;
        }

        public async Task Sync()
        {
            _logger.LogInformation("Start sync job...");
            string connectionString = _config["AZURE_FILESHARE_CONNECTIONSTRING"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("database");
            DateTime date = await GetLastFileDate(containerClient, "customrank");
            if (_db.Timestamps.Count() > 0 && _db.Timestamps.First().Date >= date)
            {
                _logger.LogInformation($"Data on {date.ToString()} already exists, exit.");
                return;
            }
            await UpdateSubjectDb(containerClient);
            await UpdateSubjectEntityDb(containerClient);
            await UpdateTagsDb(containerClient);
            await UpdateRankDb(containerClient);
            await UpdateDateDb(date);
        }

        private async Task UpdateSubjectDb(BlobContainerClient blobContainerClient)
        {
            var s = _db.Subjects.Count();
            if (s != 0)
            {
                _db.Subjects.RemoveRange(_db.Subjects);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Existing subjects removed.");
            }

            DateTime lastDate = await GetLastFileDate(blobContainerClient, "subject_archive");
            //DateTime lastDate = new DateTime(2022, 4, 3);
            string lastSubjectFile = "subject_archive_" + lastDate.ToString("yyyy_MM_dd") + ".jsonlines";
            await DownloadFile(blobContainerClient, lastSubjectFile);

            _logger.LogInformation($"UpdateSubjectDb processing file {lastSubjectFile}...");

            using (var reader = new StreamReader($"./{lastSubjectFile}"))
            {
                while (reader.Peek() >= 0)
                {
                    int cnt = 0;
                    List<Subject> subjects = new List<Subject>();
                    while (reader.Peek() >= 0 && cnt <= 10000)
                    {
                        string record = reader.ReadLine().Trim();
                        Subject subject;
                        try
                        {
                            subject = JsonConvert.DeserializeObject<Subject>(record);
                        } catch
                        {
                            _logger.LogError("Cannot deserialize Subject: ", record);
                            continue;
                        }
                        subjects.Add(subject);
                        cnt++;
                    }
                    await _db.Subjects.AddRangeAsync(subjects);
                    await _db.SaveChangesAsync();
                }
            }
            _logger.LogInformation("Subjects archived successfully.");
        }

        private async Task UpdateSubjectEntityDb(BlobContainerClient blobContainerClient)
        {
            var s = _db.SubjectEntities.Count();
            if (s != 0)
            {
                _db.SubjectEntities.RemoveRange(_db.SubjectEntities);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Existing subject entities removed.");
            }

            DateTime lastDate = await GetLastFileDate(blobContainerClient, "subject_entity");
            //DateTime lastDate = new DateTime(2022, 4, 3);
            string lastSubjectFile = "subject_entity_" + lastDate.ToString("yyyy_MM_dd") + ".jsonlines";
            await DownloadFile(blobContainerClient, lastSubjectFile);

            _logger.LogInformation($"UpdateSubjectEntityDb processing file {lastSubjectFile}...");

            using (var reader = new StreamReader($"./{lastSubjectFile}"))
            {

                while (reader.Peek() >= 0)
                {
                    int cnt = 0;
                    List<SubjectEntity> subjectEnts = new List<SubjectEntity>();
                    while (reader.Peek() >= 0 && cnt <= 10000)
                    {
                        string record = reader.ReadLine().Trim();
                        SubjectEntity subjectEnt;
                        try
                        {
                            subjectEnt = JsonConvert.DeserializeObject<SubjectEntity>(record);
                        } catch
                        {
                            _logger.LogError("Cannot deserialize SubjectEntity: ", record);
                            continue;
                        }
                        subjectEnts.Add(subjectEnt);
                        cnt++;
                    }
                    await _db.SubjectEntities.AddRangeAsync(subjectEnts);
                    await _db.SaveChangesAsync();
                }
            }
            _logger.LogInformation("Subject entities archived successfully.");
        }

        private async Task UpdateTagsDb(BlobContainerClient blobContainerClient)
        {
            var s = _db.Tags.Count();
            if (s != 0)
            {
                _db.Tags.RemoveRange(_db.Tags);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Existing tags removed.");
            }

            DateTime lastDate = await GetLastFileDate(blobContainerClient, "tags");
            //DateTime lastDate = new DateTime(2022, 4, 12);
            string lastTagFile = "tags_" + lastDate.ToString("yyyy_MM_dd") + ".jsonlines";
            await DownloadFile(blobContainerClient, lastTagFile);

            _logger.LogInformation($"UpdateTagsDb processing file {lastTagFile}...");

            using (var reader = new StreamReader($"./{lastTagFile}"))
            {

                while (reader.Peek() >= 0)
                {
                    int cnt = 0;
                    List<Tag> tags = new List<Tag>();
                    while (reader.Peek() >= 0 && cnt <= 10000)
                    {
                        string record = reader.ReadLine().Trim();
                        Tag tag;
                        try
                        {
                            tag = JsonConvert.DeserializeObject<Tag>(record);
                        }
                        catch
                        {
                            _logger.LogError("Cannot deserialize Tag: ", record);
                            continue;
                        }
                        tags.Add(tag);
                        cnt++;
                    }
                    await _db.Tags.AddRangeAsync(tags);
                    await _db.SaveChangesAsync();
                }
            }
            _logger.LogInformation("Tags archived successfully.");
        }

        private async Task UpdateRankDb(BlobContainerClient blobContainerClient)
        {
            var s = _db.CustomRanks.Count();
            if (s != 0)
            {
                _db.CustomRanks.RemoveRange(_db.CustomRanks);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Existing ranks removed.");
            }

            DateTime lastDate = await GetLastFileDate(blobContainerClient, "customrank");
            string lastRankFile = "customrank_" + lastDate.ToString("yyyy_MM_dd") + ".csv";
            await DownloadFile(blobContainerClient, lastRankFile);

            _logger.LogInformation($"UpdateSubjectDb processing file {lastRankFile}...");

            using (var reader = new StreamReader($"./{lastRankFile}"))
            {
                List<CustomRank> ranks = new List<CustomRank>();
                reader.ReadLine();
                while (reader.Peek() >= 0)
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
                await _db.CustomRanks.AddRangeAsync(ranks);
                await _db.SaveChangesAsync();
            }
            _logger.LogInformation("Ranks archived successfully.");
        }

        private async Task UpdateDateDb(DateTime date)
        {
            _db.Timestamps.RemoveRange(_db.Timestamps);
            await _db.Timestamps.AddAsync(new Timestamp { Date = date });
            await _db.SaveChangesAsync();
            _logger.LogInformation("Timestamp updated.");
        }

        private async Task<DateTime> GetLastFileDate(BlobContainerClient containerClient, string prefix)
        {
            _logger.LogInformation($"Getting latest date of {prefix}.");
            string connectionString = _config["AZURE_FILESHARE_CONNECTIONSTRING"];
            Regex dateRegex = new Regex(@"(\d+)_(\d+)_(\d+)", RegexOptions.Compiled);
            // Download subject and ranking file
            DateTime date = new DateTime(2021, 1, 1);
            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                var match = dateRegex.Match(blobItem.Name);
                if (match.Success)
                {
                    DateTime newdate = new DateTime(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value));
                    if (newdate.CompareTo(date) > 0)
                    {
                        date = newdate;
                    }
                }
            }
            _logger.LogInformation($"Latest date of {prefix} is {date.ToString()}");
            return date;
        }

        private async Task DownloadFile(BlobContainerClient containerClient, string filename)
        {
            _logger.LogInformation($"Download {filename} to working directory.");
            BlobClient file = containerClient.GetBlobClient(filename);
            if (!File.Exists($"./{filename}"))
            {
                using (FileStream stream = File.OpenWrite($"./{filename}"))
                {
                    await file.DownloadToAsync(stream);
                }
            }
            else
            {
                _logger.LogInformation($"File {filename} already exists.");
            }
        }
    }

    public class DbUpdateService : BackgroundService
    {
        private Timer _timer;
        
        private readonly ILogger<DbUpdateService> _logger;
        private readonly IServiceProvider _service;

        public DbUpdateService(IServiceProvider service, ILogger<DbUpdateService> logger)
        {
            _service = service;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Set up db update cron job.");

            _timer = new Timer(UpdateDatabase, null, TimeSpan.Zero,
                TimeSpan.FromHours(3));

            return Task.CompletedTask;
        }

        private async void UpdateDatabase(object state)
        {
            using (var scope = _service.CreateScope())
            {
                var scopedProcessingService =
                scope.ServiceProvider
                    .GetRequiredService<ISyncService>();

                await scopedProcessingService.Sync();
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Tear down db update cron job.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
