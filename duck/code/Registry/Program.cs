using duck.code.RegistryBackuper.registry.backup;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace duck.code.RegistryBackuper
{
    public static class Program
    {
        static WebApplication? app;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            app = builder.Build();

            
            // Configure the HTTP request pipeline.

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles(); // Enable serving static files

            app.UseRouting();

            app.MapGet("/", async context =>
            {
                var webRootPath = builder.Environment.WebRootPath;
                var duck = await File.ReadAllTextAsync(Path.Combine(webRootPath, "resources/duck.ascii"));
                var logo = await File.ReadAllTextAsync(Path.Combine(webRootPath, "resources/logo.ascii"));
                var htmlContent = await File.ReadAllTextAsync(Path.Combine(webRootPath, "index.html"));

                // Replace placeholder with ASCII art
                htmlContent = htmlContent.Replace("<!-- duck.ascii -->", duck);
                htmlContent = htmlContent.Replace("<!-- logo.ascii -->", logo);

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(htmlContent);
            });
            
            // Define endpoints for buttons
            app.MapGet("/backup", async context =>
            {
                await context.Response.WriteAsync(RegistryBackup.BackupFullRegistry().ToString());
            });
            app.MapGet("/folder", async context =>
            {
                await context.Response.WriteAsync(RegistryBackup.OpenBackupDirectory().ToString());
            });
            app.MapGet("/closeserver", async context =>
            {
                CloseServer();
                await context.Response.WriteAsync(true.ToString());
            });

            OpenBrowserWithDelay(GetApplicationUrl(builder));
            app.Run();
        }

        private static void CloseServer()
        {
            Thread thread = new(async () =>
            {
                Thread.Sleep(1000); // 1-second delay
                if (app != null)
                    await app.StopAsync();
            });
            thread.Start();
        }

        private static async void OpenBrowserWithDelay(string url)
        {
            await Task.Delay(1000); // 1-second delay
            OpenBrowser(url);
        }

        private static string GetApplicationUrl(WebApplicationBuilder builder)
        {
            // Assuming using Kestrel and default settings
            var urls = builder.Configuration["ASPNETCORE_URLS"]?.Split(';') ?? new string[] { "http://localhost:5000" };
            return urls.FirstOrDefault() ?? "http://localhost:5000";
        }

        private static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Failed to open browser: {ex.Message}");
            }
        }
    }
}
