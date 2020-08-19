namespace SampleRewardsLookupService.Services
{
    using System;
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Enums;
    using SampleRewardsLookupService.Models;

    public interface IRewardsInfoRetriever
    {
        /// <summary>
        /// A property to differentiate various implementations of IRewardsInfoRetriever. This is needed for dependency injection.
        /// </summary>
        PreferredDataSource Type { get; }

        /// <summary>
        /// Retrieves customer's rewards profile
        /// </summary>
        /// <param name="customerId">Loyalty Cusotmer ID</param>
        /// <returns>CustomerRewardsProfile object which contains customer's Rewards profile</returns>
        Task<CustomerRewardsProfile> GetRewardsInfoAsync(Guid customerId);
    }
}