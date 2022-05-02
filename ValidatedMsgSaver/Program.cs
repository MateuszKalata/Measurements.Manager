using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
namespace ValidatedMsgSaver
{
    class Program
    {
		public static IConfiguration configuration;
		static int Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console(Serilog.Events.LogEventLevel.Debug, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
				.MinimumLevel.Debug()
				.Enrich.FromLogContext().CreateLogger();

			try
			{
				MainTask(args).Wait();
				return 0;
			}
			catch
			{
				return 1;
			}

		}

		static async Task MainTask(string[] args)
		{
			Log.Information("Creating service collection");
			ServiceCollection serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);

			Log.Information("Building service provider");
			IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();


			try
			{
				Log.Information($"Starting ValidatedMsgSaver service");
				serviceProvider.GetService<MeasurementsSaver>().ConsumeMeasurements();
				Log.Information("Ending service");
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Error running service");
				throw ex;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
		private static void ConfigureServices(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
			{
				builder
					.AddSerilog(dispose: true);
			}));

			serviceCollection.AddLogging();

			configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
				.AddJsonFile("appsettings.json", false)
				.Build();

			serviceCollection.AddSingleton<IConfiguration>(configuration);

			serviceCollection.AddTransient<MeasurementsSaver>();

		}
    }
}
