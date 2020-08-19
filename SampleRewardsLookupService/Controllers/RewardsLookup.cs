namespace SampleRewardsLookupService.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using SampleRewardsLookupService.Constants;
    using SampleRewardsLookupService.Enums;
    using SampleRewardsLookupService.Models;
    using SampleRewardsLookupService.Services;
    using SampleRewardsLookupService.Utilities;

    /// <summary>
    /// Rewards Lookup API controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsLookup : ControllerBase
    {
        private IRewardsLookupOrchestrator orchestrator;
        private ILogger<RewardsLookup> logger;
        private IConfiguration configuration;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="orchestrator">Orchestrator for routing lookup requests and handles fallback operations when error occurs.</param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Application config</param>
        public RewardsLookup(IRewardsLookupOrchestrator orchestrator, ILogger<RewardsLookup> logger, IConfiguration configuration)
        {
            this.orchestrator = orchestrator.ThrowIfNull("orchestrator");
            this.logger = logger.ThrowIfNull("logger");
            this.configuration = configuration.ThrowIfNull("configuration");
        }

        /// <summary>
        /// Rewards Lookup API
        /// Signature: GET api/<RewardsLookupController>/[GUID]
        /// </summary>
        /// <param name="customerId">Loyalty customer ID</param>
        /// <param name="preferredDataSource">
        /// Indicates which primary data source to use for retrieving data.
        /// If the service failed to retrieve data from preferred data source, this API will attempt to
        /// get data from the backup data source.
        /// </param>
        /// <returns>CustomerRewardsProfile object which contains customer's rewards information.</returns>
        [HttpGet("{customerId}")]
        public async Task<CustomerRewardsProfile> GetCustomerRewardsProfileAsync(Guid customerId, [FromHeader] PreferredDataSource preferredDataSource)
        {
            logger.LogInformation($"Request lookup request for {LoyaltyConstants.CUSTOMER_ID}:{customerId}");

            var result = await orchestrator.GetRewardsInfoAsync(customerId, preferredDataSource);

            logger.LogInformation($"Ready to respond to lookup request for {LoyaltyConstants.CUSTOMER_ID}:{customerId}");

            return result;
        }
    }
}