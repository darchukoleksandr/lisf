using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lisf.Models;
using Newtonsoft.Json;

namespace lisf
{
    static class Analyzer
    {
        public static void SearchNewClaim()
        {
            var claimReports = new List<ClaimReport>();

            foreach (var file in Directory.GetFiles("data/Armageddon-4").TakeLast(2))
            {
                var jsonReport = File.ReadAllText(file);
                var deserializedReport = JsonConvert.DeserializeObject<ClaimReport>(jsonReport);
                claimReports.Add(deserializedReport);
            }

            if (claimReports[0].Claims.Count() < claimReports[1].Claims.Count())
            {
                Console.WriteLine($"Founded claim +{claimReports[0].Claims.Count() - claimReports[1].Claims.Count()}");
            }
        }

        public static void SearchDescreatingClaim()
        {
            var claimReports = new List<ClaimReport>();

            foreach (var file in Directory.GetFiles("data/Armageddon-4").TakeLast(2))
            {
                var jsonReport = File.ReadAllText(file);
                var deserializedReport = JsonConvert.DeserializeObject<ClaimReport>(jsonReport);
                claimReports.Add(deserializedReport);
            }

            foreach (var claim in claimReports[0].Claims)
            {
                var firstOrDefault = claimReports[1].Claims.FirstOrDefault(claim1 => claim1.Id == claim.Id);
                if (claim.Diameter > firstOrDefault?.Diameter)
                {
                    Console.WriteLine(
                        $"Clime {claim.Name} diameter is descrerated by {claim.Diameter - firstOrDefault.Diameter}, now {claim.Diameter}");
                }
                else if (firstOrDefault?.Diameter > claim.Diameter)
                {
                    Console.WriteLine(
                        $"Clime {claim.Name} diameter is inscreated by {firstOrDefault.Diameter - claim.Diameter}");
                }
            }
        }

        public static void SearchDisapearedClaim()
        {

        }
    }
}
