namespace SampleRewardsLookupService
{
    using System.Linq;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using SampleRewardsLookupService.DataAdapters;
    using SampleRewardsLookupService.Enums;
    using SampleRewardsLookupService.Services;

    /// <summary>
    /// Class for setting up the web service.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //TODO: SETUP APP CONFIG
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen();

            //Live Data Provider DI
            //TODO: CHANGE DEPENDENCY TO USE REAL CLIENT
            services.AddSingleton<IVendorXApiClient, MockVendorXApiClient>();

            //Redis Clidne DI
            //TODO: CHANGE DEPENDENCY TO USE REAL CLIENT
            services.AddSingleton<IRedisCacheClient, MockRedisCacheClient>();

            //Orchestrator DI
            services.AddSingleton<IRewardsInfoRetriever, LiveDataProvider>();
            services.AddSingleton<IRewardsInfoRetriever, CacheDataProvider>();
            services.AddSingleton<IRewardsCacheUpdater, RewardsCacheUpdater>();

            services.AddSingleton<IRewardsLookupOrchestrator>(serviceProvider =>
            {
                return new RewardsLookupOrchestrator(
                    serviceProvider.GetServices<IRewardsInfoRetriever>().Where(s => s.Type == PreferredDataSource.VendorXLiveData).FirstOrDefault(),
                    serviceProvider.GetServices<IRewardsInfoRetriever>().Where(s => s.Type == PreferredDataSource.RedisCacheData).FirstOrDefault(),
                    serviceProvider.GetRequiredService<IRewardsCacheUpdater>(),
                    serviceProvider.GetRequiredService<ILogger<RewardsLookupOrchestrator>>(),
                    serviceProvider.GetRequiredService<IConfiguration>());
            });
        }

        /// <summary>
        /// This method gets called by the runtime, after ConfigureServices(...). Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // TODO: SETUP GENERAL EXCEPTION HANDLER AND LOG EXCEPTIONS
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "RewardLookup API"); });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}