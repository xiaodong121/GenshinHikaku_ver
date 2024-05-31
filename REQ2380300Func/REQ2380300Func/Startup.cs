using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text;
using System;
using Microsoft.Extensions.Azure;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(REQ2380300Func.Startup))]
namespace REQ2380300Func
{
    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        public Startup()
        {
            // ShiftJIs対抗コード
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // ログファイル吐出し設定 local時のみ
            var ret = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            /*
            if (ret != null && ret.StartsWith("localhost", StringComparison.CurrentCultureIgnoreCase))
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                var dir = Directory.GetCurrentDirectory();
                if (Directory.Exists(dir + "/logs/"))
                {
                    Directory.CreateDirectory(dir + "/logs/");
                }

                builder.Services.AddLogging(loggingBuilder => {
                    loggingBuilder.AddFile("logs/log_{0:yyyy}-{0:MM}-{0:dd}-{0:HHmm}.txt", fileloggerOpts => {
                        fileloggerOpts.FormatLogFileName = fName => {
                            return String.Format(fName, System.TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));
                        };
                    });
                });
            }*/
        }
    }
}
