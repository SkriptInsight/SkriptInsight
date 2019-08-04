using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;

namespace SkriptInsight.Host.Lsp
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Any(a => a.ToLower().Equals("-d")))
            {
                while (!System.Diagnostics.Debugger.IsAttached)
                {
                    await Task.Delay(100);
                }
            }

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithMinimumLogLevel(LogLevel.Error)
            );

            await server.WaitForExit;
        }
    }
}