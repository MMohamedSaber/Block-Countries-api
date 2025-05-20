
using BlockedCountries.Core;
using BlockedCountries.Core.Entities;
using BlockedCountries.Core.interfaces;
using BlockedCountries.Infrastructure.Repositories;
using BlockedCountries.Infrastructure.Repositories.Services;

namespace BlockedCountries.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<ICountryRepository,CountryRepository>();
            builder.Services.AddSingleton<IGeolocationService, GeolocationService>();
            builder.Services.AddSingleton<IAttemptLogger, AttemptLogger>();
            builder.Services.AddSingleton<ITemporalBlockRepository, TemporalBlockRepository>(); 
            
            builder.Services.AddHttpClient<IGeolocationService, GeolocationService>();
            builder.Services.AddHostedService<TemporalBlockCleanupService>();


            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options=>options.SwaggerEndpoint("/openapi/v1.json","BlockedCountry")); 
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
