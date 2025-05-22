using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutonomiNet
{
    public class Register : Client
    {
        public static async Task<string> GenerateKey(bool isOverwrite = false)
        {
            var overwrite = "";
            if(isOverwrite)
                overwrite = " --overwrite";

            var result = await ExecuteAntCliCommand($"register generate-key{overwrite}");
            return result;
        }
        public static async Task<string> Cost(string key)
        {
            var result = await ExecuteAntCliCommand($"register cost {key}");
            return result;
        }
        public static async Task<string> Create(string key, string value)
        {
            var result = await ExecuteAntCliCommand($"register create {key} {value}");

            var match = Regex.Match(result, @"Entry already exists at this address:\s*([a-f0-9]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return result = match.Groups[1].Value;
            }

            return result;
        }

        public static async Task<string> Edit(string key, string value)
        {
            var result = await ExecuteAntCliCommand($"register edit --name {key} {value}");
            var match = Regex.Match(result, @"Total cost:\s*([0-9]*\.?[0-9]+)");
            if (match.Success)
            {
                result = match.Groups[1].Value.Trim();
            }
            return result;
        }

        public static async Task<string> GetByName(string name)
        {
            var result = await ExecuteAntCliCommand($"register get --name {name}");
            //var match = Regex.Match(result, @"With value:\s*\[(\d+)\s*");
            var match = Regex.Match(result, @"With value:\s*\[(\d+)\s*\]?");
            if (match.Success)
            {
                result = match.Groups[1].Value.Trim();
            }
            return result;
        }

        public static async Task<string> List()
        {
            var result = await ExecuteAntCliCommand($"register list");
            return result;
        }
    }
}
