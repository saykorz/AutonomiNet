using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutonomiNet
{
    public class Vault: Client
    {
        public static async Task<string> Cost()
        {
            var sb = new StringBuilder();
            var result = await ExecuteAntCliCommand($"vault cost");
            return result;
        }

        public static async Task<string> Create()
        {
            var result = await ExecuteAntCliCommand($"vault create");
            return result;
        }

        
        public static async Task<string> Load()
        {
            var result = await ExecuteAntCliCommand($"vault load");
            return result;
        }

        public static async Task<string> Sync(bool force = false)
        {
            var arg = string.Empty;
            if(force)
                arg = " --force";
            var result = await ExecuteAntCliCommand($"vault sync{arg}");
            return result;
        }
    }
}
