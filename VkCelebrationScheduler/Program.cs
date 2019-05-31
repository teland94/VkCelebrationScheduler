using Microsoft.Extensions.Configuration;
using System;
using System.ServiceProcess;
using VkCelebrationScheduler;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkCelebrationScheduler.Logging;

public static class Program
{
    private static async Task Main(string[] args)
    {
        List<Account> accounts = new List<Account>();
        ISimpleLogger logger;

#if DEBUG
        logger = new SimpleConsoleLogger();
#else
        logger = new SimpleFileLogger(AppContext.BaseDirectory);
#endif

        try
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.Build();
            configuration.GetSection("accounts").Bind(accounts);

            using (var service = new VkCelebrationSchedulerService(accounts, logger))
            {
#if DEBUG
                await service.RunAsConsole(args);
#else
                ServiceBase.Run(service);
#endif
            }
        }
        catch (Exception ex)
        {
            logger.WriteException(ex);
            logger.WriteLine("=====================================================");
        }
        finally
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}