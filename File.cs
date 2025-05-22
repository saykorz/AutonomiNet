using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutonomiNet
{
    public class File: Client
    {
        public static async Task<string> Cost(string filePath)
        {
            var result = await ExecuteAntCliCommand($"file cost {filePath}");
            return result;
        }
        public static async Task<string> Upload(string filePath, bool isPrivate = false)
        {
            var isPublic = "--public";
            if (isPrivate)
            {
                isPublic = string.Empty;
            }
            var result = await ExecuteAntCliCommand($"file upload {filePath} {isPublic}");

            //var match = Regex.Match(result, @"At address:\s*([a-fA-F0-9]{64})");
            var match = Regex.Match(result, @"At address:\s*([a-fA-F0-9]+)");

            if (match.Success)
            {
                result = match.Groups[1].Value;
            }

            return result;
        }
        public static async Task<string> Download(string address, string folderPath = null)
        {
            if (string.IsNullOrEmpty(folderPath))
                folderPath = ".";
            var result = await ExecuteAntCliCommand($"file download {address} {folderPath}");
            return result;
        }

        public static async Task<string> List()
        {
            var result = await ExecuteAntCliCommand($"file list");
            return result;
        }
    }
}
