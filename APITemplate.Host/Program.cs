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
                Logger.Info("Program.cs", "Main", "Configuração da aplicação carregada, logger iniciado!");

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

                // Automaticamente acessa o swagger ao clicar no link escutado (Now listening) - Console
                app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                #endregion

                Logger.Info("Program.cs", "Main", "Todos os parâmetros foram carregados, subindo a aplicação...");

                if (useSwagger && !app.Environment.IsDevelopment())
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

                Logger.Info("Program.cs", "Main", "Requisição para finalizar recebida, parando a aplicação...");
                Logger.Info("Program.cs", "Main", "aplicação encerrada.");
            }
            catch (Exception ex)
            {
                HandleStartupError(ex);
                Console.WriteLine($"{DateTime.Now} - Erro ao iniciar a aplicação: {ex}");
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
            string errorMsg = $"{DateTime.Now} - Erro ao iniciar a aplicação: {exception.ToString()}{Environment.NewLine}";
            File.AppendAllText(file, errorMsg);
        }
    }
}
