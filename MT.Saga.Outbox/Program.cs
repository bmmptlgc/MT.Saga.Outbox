using MassTransit.Internals;
using Serilog;
using Serilog.Events;

namespace MT.Saga.Outbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureServices((hostContext, services) =>
                {
                    hostContext.HostingEnvironment.ApplicationName = typeof(Program).GetTypeName();

                    services.Configure<HostOptions>(option =>
                    {
                        option.ShutdownTimeout = TimeSpan.FromSeconds(10);
                    });
                })
                .UseSerilog((host, log) =>
                {
                    log
                        .MinimumLevel.Verbose()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                        .MinimumLevel.Override("MassTransit.EntityFrameworkCoreIntegration", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseWindowsService();
    }
}