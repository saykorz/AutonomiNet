using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutonomiNet
{
    public class Client
    {
        public static bool IsTestNetwork = false;
        public static string SecretKey { get; set; }
        public static string RegisterSigningKey { get; set; }

        public static string GetPeersFromUrl(string url)
        {
            using (var client = new WebClient())
            {
                string content = client.DownloadString(url);
                var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    sb.Append(" --peer " + line);
                }

                return sb.ToString();
            }
        }

        public static async Task<string> ExecuteAntCliCommand(string arguments, bool autoConfirm = false)
        {
            var result = new StringBuilder();
            try
            {
                var process = new Process();
                var os = Environment.OSVersion.Platform;

                string antExecutable;
                string baseArgs = "";

                if (IsTestNetwork)
                {
                    baseArgs = "--alpha --network-id 2";
                    //baseArgs = "--network-id 2 --network-contacts-url http://174.138.6.129/bootstrap_cache.json --testnet";
                }

                if (os == PlatformID.Win32NT)
                {
                    // Windows setup
                    antExecutable = $"{AppContext.BaseDirectory}ant.exe";

                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = antExecutable,// "cmd.exe",
                        Arguments = $"{baseArgs} {arguments}",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                }
                else
                {
                    // Unix / Android (Termux) setup
                    antExecutable = "/mnt/shared/Pictures/ant"; // Adjust if needed

                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "sh",
                        Arguments = $"-c \"{antExecutable} {baseArgs} {arguments}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                }

                // Common environment variables
                process.StartInfo.EnvironmentVariables["SECRET_KEY"] = SecretKey;
                process.StartInfo.EnvironmentVariables["REGISTER_SIGNING_KEY"] = RegisterSigningKey; 
                if (IsTestNetwork)
                {
                    process.StartInfo.EnvironmentVariables["EVM_NETWORK"] = "arbitrum-sepolia-test";
                }

                // Output handling
                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        result.AppendLine(args.Data);
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        result.AppendLine(args.Data);
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (os == PlatformID.Win32NT)
                {
                    await process.StandardInput.WriteLineAsync($"\"{antExecutable}\" {baseArgs} {arguments}");
                    await process.StandardInput.FlushAsync();
                    process.StandardInput.Close();
                }

                await Task.Run(() => process.WaitForExit());
            }
            catch (Exception ex)
            {
                result.AppendLine($"[ERROR] {ex.Message}");
            }

            return result.ToString();
        }



        public static async Task<string> ExecuteAntCliCommand_old(string arguments, bool autoConfirm = false)
        {
            var result = new StringBuilder();
            try
            {
                var testAnt = $"ant --local --testnet"; //$"ant --network-id 2 --network-contacts-url http://174.138.6.129/bootstrap_cache.json --testnet";
                if (!IsTestNetwork)
                {
                    //var bootstrap = GetPeersFromUrl("http://192.168.60.4:38112/bootstrap.txt");
                    testAnt = $"ant --network-id 2 --network-contacts-url http://174.138.6.129/bootstrap_cache.json --testnet"; // $"ant --network-id 2{bootstrap} --testnet";
                }
                
                var process = new Process();
                var os = Environment.OSVersion.Platform;
                if (os == PlatformID.Win32NT)
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                }
                else if (os == PlatformID.Unix || os == PlatformID.MacOSX)
                {
                    const string antPath = "/mnt/shared/Pictures/";

                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "sh",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    process.StartInfo.Arguments = $"-c \"{antPath}{testAnt} {arguments}\"";
                    

                }
                else
                {
                    throw new InvalidOperationException("Unsupported operating system.");
                }

                if (IsTestNetwork)
                {
                    process.StartInfo.EnvironmentVariables["SECRET_KEY"] = SecretKey;
                }
                else
                {
                    process.StartInfo.EnvironmentVariables["SECRET_KEY"] = SecretKey;
                    process.StartInfo.EnvironmentVariables["EVM_NETWORK"] = "arbitrum-sepolia-test";
                }
                process.StartInfo.EnvironmentVariables["REGISTER_SIGNING_KEY"] = RegisterSigningKey;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        result.AppendLine(args.Data);
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        result.AppendLine(args.Data);
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (os == PlatformID.Win32NT)
                {
                    await process.StandardInput.WriteLineAsync($"{AppContext.BaseDirectory}{testAnt} {arguments}");
                    //await process.StandardInput.WriteLineAsync("exit");
                    await process.StandardInput.FlushAsync();
                    process.StandardInput.Close();
                }
                 
                await Task.Run(() => process.WaitForExit());

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while executing the command: {ex.Message}");
            }
            return result.ToString();
        }
    }
}
