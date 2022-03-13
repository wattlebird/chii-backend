using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;
using System;

[assembly: FunctionsStartup(typeof(chiiv2.Startup))]

namespace chiiv2
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string SqlConnection = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddDbContext<BangumiContext>(
                options => options.UseSqlServer(SqlConnection));
        }
    }

    public class BloggingContextFactory : IDesignTimeDbContextFactory<BangumiContext>
    {
        public BangumiContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BangumiContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));

            return new BangumiContext(optionsBuilder.Options);
        }
    }

}
