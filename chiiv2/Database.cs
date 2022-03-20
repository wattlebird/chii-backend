using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace chiiv2
{
    public class Database
    {
        private readonly BangumiContext _context;
        public Database(BangumiContext context)
        {
            _context = context;
        }

        [FunctionName("sync")]
        [StorageAccount("IkelyConnectionString")]
        public void Sync([BlobTrigger("database/{name}")] Stream stream, [Blob("database")] BlobContainerClient containerClient, string name, ILogger log)
        {
            if (name.StartsWith("customrank") && name.EndsWith("csv"))
            {
                log.LogInformation($"Function Sync processing file {name}...");
                name = name.Split('.')[0];
                string[] vs = name.Split('_');
                DateTime date = new DateTime(Int32.Parse(vs[1]), Int32.Parse(vs[2]), Int32.Parse(vs[3]));
                if (_context.Timestamps.Count() > 0 && _context.Timestamps.First().Date > date)
                {
                    log.LogInformation($"Data on {date.ToString()} already exists, exit.");
                    return;
                }

                // Removing existing data in db
                int count = _context.CustomRanks.Count();
                if (count > 0)
                {
                    _context.CustomRanks.RemoveRange(_context.CustomRanks);
                    _context.SaveChanges();
                }
                count = _context.Tags.Count();
                if (count > 0)
                {
                    _context.Tags.RemoveRange(_context.Tags);
                    _context.SaveChanges();
                }
                count = _context.SubjectEntities.Count();
                if (count > 0)
                {
                    _context.SubjectEntities.RemoveRange(_context.SubjectEntities);
                    _context.SaveChanges();
                }
                count = _context.Subjects.Count();
                if (count > 0)
                {
                    _context.Subjects.RemoveRange(_context.Subjects);
                    _context.SaveChanges();
                }

                BlobClient blobClient = containerClient.GetBlobClient($"subject_archive_{vs[1]}_{vs[2]}_{vs[3]}.jsonlines");
                SyncSubject(blobClient.OpenRead(), blobClient.Name, log);
                blobClient = containerClient.GetBlobClient($"subject_entity_{vs[1]}_{vs[2]}_{vs[3]}.jsonlines");
                SyncSubjectEntity(blobClient.OpenRead(), blobClient.Name, log);
                blobClient = containerClient.GetBlobClient($"tags_{vs[1]}_{vs[2]}_{vs[3]}.jsonlines");
                SyncTags(blobClient.OpenRead(), blobClient.Name, log);
                blobClient = containerClient.GetBlobClient($"customrank_{vs[1]}_{vs[2]}_{vs[3]}.csv");
                SyncRank(blobClient.OpenRead(), blobClient.Name, log);

                _context.Timestamps.RemoveRange(_context.Timestamps);
                _context.Timestamps.Add(new Timestamp { Date = date });
                _context.SaveChanges();
                log.LogInformation($"Timestamp updated.");
            }
        }

        private void SyncRank(Stream stream, string name, ILogger log)
        {
            if (name.StartsWith("customrank") && name.EndsWith("csv"))
            {
                log.LogInformation($"Function SyncRank processing file {name}...");

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

                _context.CustomRanks.AddRange(ranks);
                _context.SaveChanges();
                log.LogInformation($"Customrank updated.");
            }
        }

        private void SyncSubject(Stream stream, string name, ILogger log)
        {
            if (name.StartsWith("subject_archive") && name.EndsWith("jsonlines"))
            {
                log.LogInformation($"Function SyncSubject processing file {name}...");

                using (var reader = new StreamReader(stream))
                {
                    List<Subject> subjects = new List<Subject>();
                    while (reader.Peek() >= 0)
                    {
                        string record = reader.ReadLine().Trim();
                        Subject subject = JsonConvert.DeserializeObject<Subject>(record);
                        subjects.Add(subject);
                    }

                    _context.Subjects.AddRange(subjects);
                    _context.SaveChanges();
                }
                log.LogInformation($"{_context.Subjects.Count()} subject records saved.");
            }
        }

        private void SyncSubjectEntity(Stream stream, string name, ILogger log)
        {
            if (name.StartsWith("subject_entity") && name.EndsWith("jsonlines"))
            {
                log.LogInformation($"Function SyncSubjectEntity processing file {name}...");

                using (var reader = new StreamReader(stream))
                {
                    List<SubjectEntity> subjectEntities = new List<SubjectEntity>();
                    while (reader.Peek() >= 0)
                    {
                        string record = reader.ReadLine().Trim();
                        SubjectEntity subjectEntity = JsonConvert.DeserializeObject<SubjectEntity>(record);
                        subjectEntities.Add(subjectEntity);
                    }

                    _context.SubjectEntities.AddRange(subjectEntities);
                    _context.SaveChanges();
                }
                log.LogInformation($"{_context.SubjectEntities.Count()} subject entity records saved.");
            }
        }

        private void SyncTags(Stream stream, string name, ILogger log)
        {
            if (name.StartsWith("tags") && name.EndsWith("jsonlines"))
            {
                log.LogInformation($"Function SyncTags processing file {name}...");

                using (var reader = new StreamReader(stream))
                {
                    List<Tag> tags = new List<Tag>();
                    while (reader.Peek() >= 0)
                    {
                        string record = reader.ReadLine().Trim();
                        Tag tag = JsonConvert.DeserializeObject<Tag>(record);
                        tags.Add(tag);
                    }

                    _context.Tags.AddRange(tags);
                    _context.SaveChanges();
                }
                log.LogInformation($"{_context.Tags.Count()} tag records saved.");
            }
        }
    }
}
