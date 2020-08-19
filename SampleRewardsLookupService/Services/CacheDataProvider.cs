namespace SampleRewardsLookupService.Services
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using SampleRewardsLookupService.Constants;
    using SampleRewardsLookupService.DataAdapters;
    using SampleRewardsLookupService.Enums;
    using SampleRewardsLookupService.Models;
    using SampleRewardsLookupService.Utilities;

    /// <summary>
    /// A service for retrieving and processing data from Loyalty Cache.
    /// </summary>
    public class CacheDataProvider : IRewardsInfoRetriever
    {
        private IRedisCacheClient client;
        private ILogger<CacheDataProvider> logger;
        private IConfiguration configuration;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="client">Cache client.</param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Application config.</param>
        public CacheDataProvider(IRedisCacheClient client, ILogger<CacheDataProvider> logger, IConfiguration configuration)
        {
            this.client = client.ThrowIfNull("client");
            this.logger = logger.ThrowIfNull("logger");
            this.configuration = configuration.ThrowIfNull("configuration");
        }

        /// <summary>
        /// A property to differentiate various implementations of IRewardsInfoRetriever. This is needed for dependency injection.
        /// </summary>
        public PreferredDataSource Type
        {
            get
            {
                return PreferredDataSource.RedisCacheData;
            }
        }

        /// <summary>
        /// Retrieves customer's rewards profile
        /// </summary>
        /// <param name="customerId">Loyalty Cusotmer ID</param>
        /// <returns>CustomerRewardsProfile object which contains customer's Rewards profile</returns>
        public async Task<CustomerRewardsProfile> GetRewardsInfoAsync(Guid customerId)
        {
            logger.LogInformation($"Lookup reqest for {LoyaltyConstants.CUSTOMER_ID}:{customerId} is forwarded to cache data provider.");

            var sw = new Stopwatch();
            sw.Start();

            var result = await client.GetRewardsDataAsync(customerId).ConfigureAwait(false);

            // If we have a cache miss, return right away.
            if (result == null)
                return result;

            // Otherwise, decorate the result object before returning to the caller.
            result.CustomerId = customerId;
            result.DataSource = LoyaltyConstants.REWARDS_CACHE;
            result.RetrivalTime = (int)(sw.Elapsed.TotalMilliseconds);

            logger.LogInformation($"Lookup reqest for {LoyaltyConstants.CUSTOMER_ID}:{customerId} ready from cache data provider.");

            return result;
        }
    }
}