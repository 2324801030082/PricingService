using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;
using PricingService.Data;
using PricingService.Domain;

namespace PricingService;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateWebHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDiscoveryClient(Configuration);

        services.AddDbContext<PricingDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDataStore, DbDataStore>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        services.AddControllers()
            .AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PricingDbContext dbContext)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        dbContext.Database.Migrate();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
