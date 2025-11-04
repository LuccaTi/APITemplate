
using Microsoft.Extensions.Hosting;
using Serilog;
using Service.API.Controllers;
using Service.API.Interfaces;
using Service.API.Logging;
using Service.API.Services;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace API.Host
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

                string logDirectory = Path.Combine(apiBaseDirectory, builder.Configuration["API:LogDirectory"] ?? "logs").Replace(@"/", "\\");
                Logger.InitLogger(logDirectory);
                Logger.Info("Program.cs", "Main", "Configuração da API carregada, logger da API iniciado!");

                builder.Host.UseSerilog();

                builder.Services.AddScoped<IApiService, ApiService>();

                builder.Services.AddControllers();
                bool useSwagger = Convert.ToBoolean(builder.Configuration["API:UseSwagger"]);
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

                // Automaticamente acessa o swagger ao clicar no link escutado (Now listening) - Console
                app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                #endregion

                Logger.Info("Program.cs", "Main", "Todos os parâmetros foram carregados, subindo a API...");

                if (useSwagger)
                {
                    // Troca para sempre usar https
                    app.Lifetime.ApplicationStarted.Register(() =>
                    {
                        var address = app.Urls.FirstOrDefault();
                        if (address != null && address.StartsWith("http://"))
                        {
                            address = address.Replace("http://", "https://");
                        }
                        var swaggerUrl = $"{address}/swagger";
                        Logger.Info("Program.cs", "Main", $"===== Abrindo navegador em: {swaggerUrl} =====");

                        OpenBrowser(swaggerUrl);
                    });
                }
                else
                {
                    Logger.Info("Program.cs", "Main", $"Swagger foi desabilitado!");
                }

                app.Run();

                Logger.Info("Program.cs", "Main", "Requisição para finalizar recebida, parando a API...");
                Logger.Info("Program.cs", "Main", "API encerrada.");
            }
            catch (Exception ex)
            {
                HandleStartupError(ex);
                Console.WriteLine($"{DateTime.Now} - Erro ao iniciar a API: {ex}");
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
                Logger.Error("Program.cs", "OpenBrowser", $"Erro ao abrir navegador: {ex.Message}");
                throw;
            }
        }

        private static void HandleStartupError(Exception exception)
        {
            // Cria um arquivo devido a chance do Logger não ter sido iniciado

            string apiDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string fatalErrorDirectory = Path.Combine(apiDirectory, "StartupErrors");
            if (!Directory.Exists(fatalErrorDirectory))
                Directory.CreateDirectory(fatalErrorDirectory);

            string timeStamp = DateTime.Now.Date.ToString("yyyyMMdd");
            string file = Path.Combine(fatalErrorDirectory, $"{timeStamp}_ERROR_.txt");
            string errorMsg = $"{DateTime.Now} - Erro ao iniciar a API: {exception.ToString()}{Environment.NewLine}";
            File.AppendAllText(file, errorMsg);
        }
    }
}
