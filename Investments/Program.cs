using Investments;
using Investments.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));
}