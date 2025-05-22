using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutonomiNet
{
    public class Wallet : Client
    {
        public static async Task<string> Create(string password = null)
        {
            var result = await ExecuteAntCliCommand($"wallet create {GetPassword(password)}");
            return result;
        }

        public static async Task<string> Import(string privateKey, string password = null)
        {
            var sb = new StringBuilder();
            var result = await ExecuteAntCliCommand($"wallet import {privateKey} {GetPassword(password)}");
            return result;
        }
        public static async Task<string> Export()
        {
            var result = await ExecuteAntCliCommand($"wallet export");
            return result;
        }

        public static async Task<string> Ballance()
        {
            var result = await ExecuteAntCliCommand($"wallet balance");
            return result;
        }

        private static string GetPassword(string password)
        {
            var sb = new StringBuilder();
            if (string.IsNullOrEmpty(password))
            {
                sb.Append("--no-password");
            }
            else
            {
                sb.Append($"--password {password}");
            }
            return sb.ToString();
        }
    }
}
