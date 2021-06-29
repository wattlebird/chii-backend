using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dbmgr
{
    class Program
    {
        static async Task<DateTime> get_last_file_date(string fileshare, string dir, string prefix)
        {
            Console.WriteLine($"Getting latest date of {dir}.");
            string connectionString = Environment.GetEnvironmentVariable("FILESHARE_CONNECTION_STRING");
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
            Console.WriteLine($"Latest date of {dir} is {date.ToString()}");
            return date;
        }

        static async Task download_file(string fileshare, string dir, string filename)
        {
            Console.WriteLine($"Download {filename} to working directory.");
            string connectionString = Environment.GetEnvironmentVariable("FILESHARE_CONNECTION_STRING");
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
                Console.WriteLine($"File {filename} already exists.");
            }
        }

        static async Task init_subject_db()
        {
            string fileShareConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            const string shareName = @"bangumi-publish";
            DateTime lastDate = await get_last_file_date(shareName, "subject", "subject_");
            string lastSubjectFile = "subject_"+lastDate.ToString("yyyy_MM_dd")+".tsv";
            await download_file(shareName, "subject", lastSubjectFile);

            StreamReader fSubject = new StreamReader($"./{lastSubjectFile}");

            Console.WriteLine("Reading all subjects...");
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

            using (var db = new BangumiContext())
            {
                var s = db.Subjects.Count();
                if (s != 0)
                {
                    db.Subjects.RemoveRange(db.Subjects);
                }
                db.AddRange(subjects);
                db.SaveChanges();
            }
        }

        static async Task init_tag_db()
        {
            string fileShareConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            const string shareName = @"bangumi-publish";
            DateTime lastDate = await get_last_file_date(shareName, "tags", "customtags_");
            string lastTagFile = "customtags_" + lastDate.ToString("yyyy_MM_dd") + ".tsv";
            await download_file(shareName, "tags", lastTagFile);

            StreamReader fTag = new StreamReader($"./{lastTagFile}");

            Console.WriteLine("Reading all tags...");
            string line = fTag.ReadLine(); // skip header
            List<Tag> tags = new List<Tag>();
            while ((line = fTag.ReadLine()) != null)
            {
                var items = line.Split('\t');
                if (String.IsNullOrWhiteSpace(items[1]) || items[1].Length > 256) continue;
                tags.Add(new Tag
                {
                    SubjectId = Convert.ToInt32(items[0]),
                    Content = items[1],
                    TagCount = Convert.ToInt32(items[2]),
                    UserCount = Convert.ToInt32(items[3]),
                    Confidence = Convert.ToDouble(items[4])
                });
            }

            using (var db = new BangumiContext())
            {
                var s = db.Tags.Count();
                if (s != 0)
                {
                    db.Tags.RemoveRange(db.Tags);
                }
                db.Tags.AddRange(tags);
                db.SaveChanges();
            }
        }

        static async Task init_rank_db()
        {
            string fileShareConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            const string shareName = @"bangumi-publish";
            DateTime lastDate = await get_last_file_date(shareName, "ranking", "customrank_");
            string lastRankFile = "customrank_" + lastDate.ToString("yyyy_MM_dd") + ".csv";
            await download_file(shareName, "ranking", lastRankFile);

            StreamReader fRank = new StreamReader($"./{lastRankFile}");

            Console.WriteLine("Reading all ranks...");
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

            using (var db = new BangumiContext())
            {
                var s = db.CustomRanks.Count();
                if (s != 0)
                {
                    db.CustomRanks.RemoveRange(db.CustomRanks);
                }
                db.CustomRanks.AddRange(ranks);
                db.SaveChanges();
            }
        }

        static void Main(string[] args)
        {
            init_rank_db().Wait();
        }
    }
}
