using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json.Serialization;
using CrystalQuartz.AspNetCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;
using FluentMigrator.Runner;
using log4net;
using log4net.Config;
using RmMiddleware.Migrations;

namespace RmMiddleware;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var log = CreateLog4NetLogger();
        log.Info("RMIG is starting up. Adding logger to DI as a singleton reference.");
        builder.Services.AddSingleton(log);
        log.Info(
            "Has added log4net as a singleton reference. It is now available for the controllers etc.  Will proceed to add the standard DI tooling.");

        var migrationConnectionString =
            $"Data Source={Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                        ?? throw new InvalidOperationException("Invalid SQLLite directory available.")
                , "configuration.db")}";

        log.Info($"About to run migrations with migration connection string {migrationConnectionString}.");

        builder.Services.AddFluentMigratorCore()
            .ConfigureRunner(r => r.AddSQLite()
                .WithGlobalConnectionString(migrationConnectionString)
                .ScanIn(typeof(Anchor).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        log.Info("Concluded migrations. Will write out the important environment variables used in startup.");

        log.Info($"BAYAN_HTTP_ENDPOINT_JWT:{Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_JWT")}");
        log.Info($"BAYAN_HTTP_ENDPOINT_REPORT:{Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_JWT")}");
        log.Info($"BAYAN_HTTP_USER:{Environment.GetEnvironmentVariable("BAYAN_HTTP_USER")}");
        log.Info($"CREDITLENS_HTTP_ENDPOINT:{Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT")}");
        log.Info($"CREDITLENS_HTTP_USER:{Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER")}");

        log.Info("Done writing out important environment variables used in startup.");

        builder.Services.AddRazorPages();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RM API", Version = "v1" });
            //c.ExampleFilters();
            c.EnableAnnotations();
        });

        log.Info(
            "About to add the http handler. This is legacy code as we use a static method for calls.  Might revisit this in the future.");

        builder.Services.AddHttpClient("creditlens", httpClient =>
        {
            httpClient.BaseAddress =
                new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        log.Info(
            "Setting up the default contract resolver in the application which improves serialisation performance in general.");

        // JSON Serializer
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
        );

        log.Info("Added json contract resolver to DI.  Proceeding to Fluent Migration runner.");

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        log.Info("Executed migrations.  Will add more general DI for asp.net.");

        // Enable CORS
        app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

        log.Info("Executed migrations.  Will add CORS.");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode == 404 &&
                !Path.HasExtension(context.Request.Path.Value))
            {
                context.Request.Path = "/client-app/index.html";
                await next();
            }
        });

        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();

        log.Info("DI set up and will start the application.");

        await app.RunAsync();

        log.Info("Started application.");
    }

    private static ILog CreateLog4NetLogger()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        var log = LogManager.GetLogger(typeof(ILog));
        return log;
    }

    private static IScheduler CreateScheduler()
    {
        var schedulerBuild = SchedulerBuilder.Create("RmScheduler", "RiskMatrix Scheduler");
        var schedulerFactory = schedulerBuild.Build();
        //var schedulerFactory = new StdSchedulerFactory();
        var scheduler = schedulerFactory.GetScheduler().Result;
        var types = GetImplementations<IJob>();
        foreach (var t in types)
        {
            var jobDetail = JobBuilder.Create(t).WithIdentity($"{t.Name}Job").StoreDurably().Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{t.Name}Trigger")
                .Build();
            scheduler.ScheduleJob(jobDetail, trigger);
        }

        scheduler.Start();
        return scheduler;
    }

    private static List<Type> GetImplementations<TInterface>(string assemblyFilter = "")
    {
        var assemblies = new List<Assembly>();
        var types = new List<Type>();
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

        if (Directory.Exists(Path.Combine(path, "Jobs")))
        {
            foreach (var dll in Directory.GetFiles(path, "Jobs\\*.dll"))
                assemblies.Add(Assembly.LoadFile(dll));

            if (!string.IsNullOrWhiteSpace(assemblyFilter))
                assemblies = assemblies.Where(a => a.FullName?.Contains(assemblyFilter) ?? false).ToList();

            types = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(TInterface).IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false })
                .ToList();
        }

        return types;
    }
}