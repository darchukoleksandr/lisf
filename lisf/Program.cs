namespace lisf
{
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Configuration;

    internal class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            var servers = Configuration.GetSection("servers")
                .GetChildren()
                .Select(section => new
                {
                    Name = section.GetSection("name").Value,
                    Url = section.GetSection("url").Value
                }).ToArray();

            foreach (var server in servers)
            {
                var report = new Parser(server.Url).DownloadData().Result;
                new ReportStore(server.Name).Save(report).Wait();
            }

            Analyzer.SearchDisapearedClaim();
            Analyzer.SearchNewClaim();
            Analyzer.SearchDescreatingClaim();
        }
    }
}
