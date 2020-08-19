namespace SampleRewardsLookupService.Services
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using SampleRewardsLookupService.Constants;
    using SampleRewardsLookupService.Enums;
    using SampleRewardsLookupService.Models;
    using SampleRewardsLookupService.Utilities;

    /// <summary>
    /// Orchestrator for routing lookup requests and handles fallback operations when error occurs.
    /// </summary>
    public class RewardsLookupOrchestrator : IRewardsLookupOrchestrator
    {
        private IRewardsInfoRetriever liveDataSource;
        private IRewardsInfoRetriever cacheDataSource;
        private IRewardsCacheUpdater cacheUpdater;

        private ILogger<RewardsLookupOrchestrator> logger;
        private IConfiguration configuration;

        private TimeSpan cacheDataTimeout;
        private TimeSpan liveDataTimeout;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="liveDataSource">A service which pulls data from live data source.</param>
        /// <param name="cacheDataSource">A service which pulls data from cache data source.</param>
        /// <param name="cacheUpdater">A service which backfill cache data source.</param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Application config.</param>
        public RewardsLookupOrchestrator(
            IRewardsInfoRetriever liveDataSource,
            IRewardsInfoRetriever cacheDataSource,
            IRewardsCacheUpdater cacheUpdater,
            ILogger<RewardsLookupOrchestrator> logger,
            IConfiguration configuration)
        {
            this.liveDataSource = liveDataSource;
            this.cacheDataSource = cacheDataSource;
            this.cacheUpdater = cacheUpdater;

            this.logger = logger;
            this.configuration = configuration;

            //TODO: SET UP APP CONFIG
            this.liveDataTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            //this.liveDataTimeout = this.configuration.GetValue<TimeSpan>("LiveDataTimeout");
            this.cacheDataTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            //this.cacheDataTimeout = this.configuration.GetValue<TimeSpan>("CacheDataTimeout");
        }

        /// <summary>
        /// Retrieve customer's rewards profile.
        /// </summary>
        /// <param name="customerId">Loyalty customer ID</param>
        /// <param name="preferredDataSource">Indicates which data source to retrieve data from.</param>
        /// <returns>CustomerRewardsProfile object which contains customer's rewards information.</returns>
        public async Task<CustomerRewardsProfile> GetRewardsInfoAsync(Guid customerId, PreferredDataSource preferredDataSource = PreferredDataSource.VendorXLiveData)
        {
            if (preferredDataSource == PreferredDataSource.VendorXLiveData)
            {
                return await GetLiveDataAsync(customerId).ConfigureAwait(false);
            }
            else if (preferredDataSource == PreferredDataSource.RedisCacheData)
            {
                return await GetCacheDataAsync(customerId).ConfigureAwait(false);
            }
            else
            {
                throw new NotImplementedException("Currently preferred data source is limited to live vendor data source and cache data source.");
            }
        }

        private async Task<CustomerRewardsProfile> GetCacheDataAsync(Guid customerId)
        {
            CustomerRewardsProfile result = null;
            try
            {
                logger.LogInformation($"Retrieving data for {LoyaltyConstants.CUSTOMER_ID}:{customerId} from cache data source.");
                result = await cacheDataSource.GetRewardsInfoAsync(customerId).TimeoutAfter(this.cacheDataTimeout);
                logger.LogInformation($"Received data from cache data source for {LoyaltyConstants.CUSTOMER_ID}:{customerId}.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception encountered while trying to get data from cache.");
            }

            // If we have a cache miss. Fetch data from live source and backfill cache.
            // TODO: ADD A CONFIG FLAG HERE SO WE CAN CONTROLL THE FALLBACK BEHAVIOR, SO THAT THE FALLBACK MECHANISM WILL NOT BRING DOWN OUR VENDOR'S DB.
            //       Without fallback, return will be null with cache miss.
            if (result == null)
            {
                logger.LogWarning($"Unable to retrieve data for {LoyaltyConstants.CUSTOMER_ID}:{customerId} from cache. Attempt to retrieve data from live data source and backfill cache.");
                try
                {
                    logger.LogInformation($"Retrieving data for {LoyaltyConstants.CUSTOMER_ID}:{customerId} from live data source.");
                    result = await liveDataSource.GetRewardsInfoAsync(customerId).TimeoutAfter(this.liveDataTimeout);
                    logger.LogInformation($"Received data from live data source for {LoyaltyConstants.CUSTOMER_ID}:{customerId}. Backfilling cache..");

                    var cacheUpdateResult = await cacheUpdater.UpdateRewardsInfoAsync(result).ConfigureAwait(false);
                    logger.LogInformation($"Cache data for {LoyaltyConstants.CUSTOMER_ID}:{customerId} is backfilled:{cacheUpdateResult}");

                    return result;
                }
                catch (TimeoutException te)
                {
                    var errorMsg = $"Unable to retrieve data from live data source to backfill cache for {LoyaltyConstants.CUSTOMER_ID}:{customerId}. Timed out after {this.liveDataTimeout.TotalMilliseconds}ms.";
                    logger.LogError(te, errorMsg);

                    //TODO: POSSIBILY CREATE A CUSTOM EXCEPTION FOR BETTER TRACKING/HANDLING
                    throw new TimeoutException(errorMsg, te);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Exception encountered while trying to get data from live data source as a fallback mechanism.");
                    throw;
                }
            }

            return result;
        }

        private async Task<CustomerRewardsProfile> GetLiveDataAsync(Guid customerId)
        {
            try
            {
                logger.LogInformation($"Retrieving data for {LoyaltyConstants.CUSTOMER_ID}:{customerId} from live data source.");
                var result = await liveDataSource.GetRewardsInfoAsync(customerId).TimeoutAfter(this.liveDataTimeout);
                logger.LogInformation($"Received data from live data source for {LoyaltyConstants.CUSTOMER_ID}:{customerId}.");

                return result;
            }
            catch (TimeoutException te)
            {
                logger.LogWarning(te, $"Unable to retrieve data from live data source for {LoyaltyConstants.CUSTOMER_ID}:{customerId}. Timed out after {this.liveDataTimeout.TotalMilliseconds}ms. Retrieving data from cache data source.");
                var result = await cacheDataSource.GetRewardsInfoAsync(customerId).TimeoutAfter(this.cacheDataTimeout);

                // If we also have a cache miss..
                if (result == null)
                {
                    logger.LogError($"Unable to retrieve data for {LoyaltyConstants.CUSTOMER_ID}:{customerId} from any available data source.");
                    throw;
                }

                // If we have a cache hit
                logger.LogInformation($"Received data from live data source for {LoyaltyConstants.CUSTOMER_ID}:{customerId}.");
                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception encountered while trying to get data from live data source. Retrieving data from cache data source.");

                var result = await cacheDataSource.GetRewardsInfoAsync(customerId).TimeoutAfter(this.cacheDataTimeout);

                // If we also have a cache miss..
                if (result == null)
                {
                    logger.LogError($"Unable to retrieve data for {LoyaltyConstants.CUSTOMER_ID}:{customerId} from any available data source.");
                    throw;
                }

                // If we have a cache hit
                logger.LogInformation($"Received data from live data source for {LoyaltyConstants.CUSTOMER_ID}:{customerId}.");
                return result;
            }
        }
    }
}