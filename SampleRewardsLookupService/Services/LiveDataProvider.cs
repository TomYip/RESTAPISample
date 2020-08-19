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
    /// A service class to retrieve and transform data from vendor.
    /// </summary>
    public class LiveDataProvider : IRewardsInfoRetriever
    {
        private IVendorXApiClient client;
        private ILogger<LiveDataProvider> logger;
        private IConfiguration configuration;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="client">Http client to communicate with vendor.</param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Application config.</param>
        public LiveDataProvider(IVendorXApiClient client, ILogger<LiveDataProvider> logger, IConfiguration configuration)
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
                return PreferredDataSource.VendorXLiveData;
            }
        }

        /// <summary>
        /// Retrieves customer's rewards profile
        /// </summary>
        /// <param name="customerId">Loyalty Cusotmer ID</param>
        /// <returns>CustomerRewardsProfile object which contains customer's Rewards profile</returns>
        public async Task<CustomerRewardsProfile> GetRewardsInfoAsync(Guid customerId)
        {
            logger.LogInformation($"Lookup reqest for {LoyaltyConstants.CUSTOMER_ID}:{customerId} is forwarded to live data provider.");

            var sw = new Stopwatch();
            sw.Start();

            var vendorCustomerId = await client.GetVendorCustomerIdAsync(customerId).ConfigureAwait(false);
            var vendorResponse = await client.GetRewardsDataAsync(vendorCustomerId).ConfigureAwait(false);

            CustomerRewardsProfile result;
            try
            {
                result = TransformData(vendorResponse);
                result.CustomerId = customerId;
                result.DataSource = LoyaltyConstants.VENDOR_LIVE_DATA;
                result.RetrivalTime = (int)(sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to transform vendor's data.");
                throw;
            }

            logger.LogInformation($"Lookup reqest for {LoyaltyConstants.CUSTOMER_ID}:{customerId} ready from live data provider.");

            return result;
        }

        /// <summary>
        /// Mapper function to transform vendor's model to our model
        /// </summary>
        /// <param name="vendorData"></param>
        /// <returns></returns>
        /// <remarks>In real code, mapper libraries will be used.</remarks>
        private CustomerRewardsProfile TransformData(VendorXRewardsData vendorData)
        {
            var mappedData = new CustomerRewardsProfile()
            {
                LevelInformation = new RewardsLevel()
                {
                    Level = vendorData.Level,
                    PointsEarnedYTD = vendorData.PointsEarnedYTD,
                    PointsEarnedLifetime = vendorData.PointsEarnedLifetime
                },
                NotesInformation = vendorData.Notes
            };

            return mappedData;
        }
    }
}