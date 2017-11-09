using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Extensions;
using lisf.Models;

namespace lisf
{
    class Parser
    {
        private readonly List<Claim> _claims = new List<Claim>();
        private string _url;

        public Parser(string url)
        {
            _url = url;
        }

        public async Task<ClaimReport> DownloadData()
        {
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(_url);
            var scripts = document.QuerySelectorAll("script");
            var scriptsData = scripts.Select(element => element.TextContent).Where(s => !string.IsNullOrEmpty(s)).ToArray();

            var div = document.QuerySelectorAll("div").Where(element => element.Id == "livemap-container").ToArray();
            var claimsIds = div.Children("a").Select(element =>
                Convert.ToInt32(element.Id.Substring(6))).ToArray();
            var claimsNames = div.Children("div").Children("p").Select(element => element.TextContent).ToArray();

            var claims = claimsIds.Zip(claimsNames, (id, name) => new { Id = id, Name = name })
                .ToDictionary(arg => arg.Id, arg => arg.Name);

            var claimsData = scriptsData[0]?
                .Replace("\n", "")
                .Replace("\t", "")
                .Replace("\r", "");

            var report = CreateClaimReport(claimsData, claims);

            var serverDay = Regex.Match(scriptsData[1], "\\\"day\\\":(\\d+)", RegexOptions.Compiled).Groups[1].Value;
            var date = new DateTime().AddYears(1027).AddDays(Convert.ToInt32(serverDay) + 1);
            report.GameDateTime = date;

            return report;
        }

        private ClaimReport CreateClaimReport(string data, Dictionary<int, string> claims)
        {
            var match = Regex.Split(data, "var trash = new Opentip", RegexOptions.Compiled).Skip(1).AsParallel();
            foreach (var matchGroup in match)
            {
                var parameters = matchGroup.Split('+');
                var id = Convert.ToInt32(Regex.Match(parameters[0], "\\d+", RegexOptions.Compiled).Groups[0].Value);
                var name = claims.First(pair => pair.Key == id).Value;
                var date = Regex.Match(parameters[1], "\\d{4}-\\d{2}-\\d{2}, \\d{2}:\\d{2}", RegexOptions.Compiled).Groups[0].Value;
                var diameter = Convert.ToInt32(Regex.Match(parameters[2], "\\d+", RegexOptions.Compiled).Groups[0].Value);
                var structures = Convert.ToInt32(Regex.Match(parameters[3], "\\d+", RegexOptions.Compiled).Groups[0].Value);
                var members = new Collection<Member>();

                for (var i = 5; i < parameters.Length; i++)
                {
                    var matchs = Regex.Match(parameters[i], "<ul|<li|<img|</li|</ul", RegexOptions.Compiled);
                    if (!matchs.Success)
                    {
                        members.Add(new Member
                        {
                            Name = parameters[i].Replace("\"", "").TrimEnd()
                        });
                    }
                }

                _claims.Add(new Claim
                {
                    Id = id,
                    Name = name,
                    Created = DateTime.Parse(date),
                    Diameter = diameter,
                    Structures = structures,
                    Members = members
                });
            }

            return new ClaimReport
            {
                Claims = _claims
            };
        }
    }
}
