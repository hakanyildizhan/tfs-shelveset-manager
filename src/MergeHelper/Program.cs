using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using TFSHelper.Core;
using TFSHelper.Core.Service;

namespace MergeHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            // arguments: 
            //  last merge changeset
            //  (optional) integ. package path -> default: D:\WS\TH_HSP150001_All\src\HM\Xdd\mddp_HSP_V15_1_0238_002_Softstarter_3RW5_V15.1.0.0.xml
            //  (optional) WM5 branch path -> default: D:\WS\WM5_WinCC_HW_Work

            // output: 
            //  current changeset nr, 
            //  file difference list
            //  associated v2.0 file changes (changeset nr's) since last merge changeset.

            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var app = new Application2(serviceCollection);
            app.Start();
            Console.Write("Press Enter to exit.");
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // add version control service
            ITFSManager tfs = new TFSManager();
            IVersionControl vc = tfs.GetService<VersionControl>();
            serviceCollection.AddSingleton<IVersionControl>(vc);

            // add logging
            //ILoggerFactory _loggerFactory = new LoggerFactory();
            //_loggerFactory.AddProvider(new ConsoleLoggerProvider(new OptionsMonitor<ConsoleLoggerSettings>();

            var configureNamedOptions = new ConfigureNamedOptions<ConsoleLoggerOptions>("", null);
            var optionsFactory = new OptionsFactory<ConsoleLoggerOptions>(new[] { configureNamedOptions }, Enumerable.Empty<IPostConfigureOptions<ConsoleLoggerOptions>>());
            var optionsMonitor = new OptionsMonitor<ConsoleLoggerOptions>(optionsFactory, Enumerable.Empty<IOptionsChangeTokenSource<ConsoleLoggerOptions>>(), new OptionsCache<ConsoleLoggerOptions>());
            ILoggerFactory loggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider(optionsMonitor) }, new LoggerFilterOptions { MinLevel = LogLevel.Information });
            serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);
        }
    }
}
