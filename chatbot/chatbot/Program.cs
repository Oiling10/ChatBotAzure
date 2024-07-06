using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using chatbot.Data;
using chatbot.Models; // Para AzureSettings en chatbot.Models
using chatbot.Services; // Para otros servicios
using System;
using Azure.AI.Language.QuestionAnswering;
using Azure;

namespace chatbot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigureApplication(app);
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Usar el nombre completo del espacio de nombres para AzureSettings
            services.Configure<chatbot.Models.AzureSettings>(configuration.GetSection("AzureSettings"));

            services.AddHttpClient<IAzCognitiveIntegrator, AzCognitiveIntegrator>(client =>
            {
                var azureSettings = configuration.GetSection("AzureSettings").Get<chatbot.Models.AzureSettings>();
                if (string.IsNullOrEmpty(azureSettings.CognitiveServicesEndpoint) || string.IsNullOrEmpty(azureSettings.CognitiveServicesApiKey))
                {
                    throw new InvalidOperationException("Azure settings are not configured properly.");
                }

                client.BaseAddress = new Uri(azureSettings.CognitiveServicesEndpoint);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureSettings.CognitiveServicesApiKey);
            });

            services.AddSingleton(provider =>
            {
                var azureSettings = configuration.GetSection("AzureSettings").Get<chatbot.Models.AzureSettings>();
                return new QuestionAnsweringService(azureSettings.CognitiveServicesEndpoint, azureSettings.CognitiveServicesApiKey);
            });

            services.AddDbContext<ChatBotContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                       .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        private static void ConfigureApplication(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
