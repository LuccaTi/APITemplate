using APITemplate.Host.Interfaces;
using APITemplate.Host.Logging;
using APITemplate.Host.Services;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace APITemplate.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                #region DI Container

                string apiBaseDirectory = Path.GetDirectoryName(AppContext.BaseDirectory)!;

                builder.Configuration
                    .SetBasePath(apiBaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

                string logDirectory = Path.Combine(apiBaseDirectory, builder.Configuration["Startup:LogDirectory"] ?? "logs").Replace(@"/", "\\");
                Logger.InitLogger(logDirectory);
                Logger.Info("Program.cs", "Main", "Application configuration loaded, logger started!");

                builder.Host.UseSerilog();

                builder.Services.AddScoped<ITestService, TestService>();

                builder.Services.AddControllers();
                bool useSwagger = Convert.ToBoolean(builder.Configuration["Startup:UseSwagger"]);
                if (useSwagger)
                {
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                }

                #endregion

                var app = builder.Build();

                #region Middleware

                if (useSwagger)
                {
                    app.UseStaticFiles();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Automatically accesses swagger when clicking on the listened link (Now listening) - Console
                app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                #endregion

                Logger.Info("Program.cs", "Main", "All parameters have been loaded, starting the application...");

                if (useSwagger && !app.Environment.IsDevelopment())
                {
                    // Switch to always use https
                    app.Lifetime.ApplicationStarted.Register(() =>
                    {
                        var address = app.Urls.FirstOrDefault();
                        if (address != null && address.StartsWith("http://"))
                        {
                            address = address.Replace("http://", "https://");
                        }
                        var swaggerUrl = $"{address}/swagger";
                        Logger.Info("Program.cs", "Main", $"===== Opening browser at: {swaggerUrl} =====");

                        OpenBrowser(swaggerUrl);
                    });
                }
                else
                {
                    Logger.Info("Program.cs", "Main", $"Swagger has been disabled!");
                }

                app.Run();

                Logger.Info("Program.cs", "Main", "Request to terminate received, stopping the application...");
                Logger.Info("Program.cs", "Main", "Application terminated.");
            }
            catch (Exception ex)
            {
                HandleStartupError(ex);
                Console.WriteLine($"{DateTime.Now} - Error starting the application: {ex}");
                Environment.Exit(1);
            }
        }

        private static void OpenBrowser(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Program.cs", "OpenBrowser", $"Error opening browser: {ex.Message}");
                throw;
            }
        }

        private static void HandleStartupError(Exception exception)
        {
            // Creates a file due to the chance that Logger has not been initialized

            string apiDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string fatalErrorDirectory = Path.Combine(apiDirectory, "StartupErrors");
            if (!Directory.Exists(fatalErrorDirectory))
                Directory.CreateDirectory(fatalErrorDirectory);

            string timeStamp = DateTime.Now.Date.ToString("yyyyMMdd");
            string file = Path.Combine(fatalErrorDirectory, $"{timeStamp}_ERROR_.txt");
            string errorMsg = $"{DateTime.Now} - Error starting the application: {exception.ToString()}{Environment.NewLine}";
            File.AppendAllText(file, errorMsg);
        }
    }
}
