
using Hangfire;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Mentorea.Hubs;
using Mentorea.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace Mentorea
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDependencies(builder.Configuration);
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)
            );
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRateLimiter();
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
                        Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
                    }
                }
            });

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var method = scope.ServiceProvider.GetRequiredService<ISessionService>();

            RecurringJob.AddOrUpdate("cancel-unconfirmed-sessions", () => method.CancelUnconfirmedSessionsAsync(), "0 */2 * * *");

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseExceptionHandler();
            app.UseCors();
            app.MapHub<ChatHub>("/ChatHub");
            app.MapHealthChecks("health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }).RequireAuthorization("AdminOnly").RequireRateLimiting("default");
            app.MapControllers();

            app.Run();

        }


    }
}
