using System;
using System.IO;
using System.Threading.Tasks;
using lisf.Models;
using Newtonsoft.Json;

namespace lisf
{
    class ReportStore
    {
        private readonly string _folderName;

        public ReportStore(string serverName)
        {
            _folderName = Path.Combine("data", serverName);
            if (!Directory.Exists(_folderName))
            {
                Directory.CreateDirectory(_folderName);
            }
        }

        public async Task Save(ClaimReport report)
        {
            var fileStream = File.Create(Path.Combine(_folderName, $"{DateTime.Now:dd.MM.yyyy HH.mm.ss}.json"));

            using (var streamWriter = new StreamWriter(fileStream))
            {
                await streamWriter.WriteLineAsync(JsonConvert.SerializeObject(report));
            }
        }
    }
}
