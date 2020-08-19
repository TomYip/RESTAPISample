namespace SampleRewardsLookupService.DataAdapters
{
    using System;
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Models;

    /// <summary>
    /// Redis Client for retrieving data from Loyalty's Redis cache.
    /// </summary>
    public interface IRedisCacheClient
    {
        /// <summary>
        /// Retrieve Rewards information from cache.
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>CustomerRewardsProfile object which contains customer's rewards information.</returns>
        Task<CustomerRewardsProfile> GetRewardsDataAsync(Guid customerId);
    }
}