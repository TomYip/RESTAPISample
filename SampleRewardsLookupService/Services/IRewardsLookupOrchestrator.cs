namespace SampleRewardsLookupService.Services
{
    using System;
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Enums;
    using SampleRewardsLookupService.Models;

    public interface IRewardsLookupOrchestrator
    {
        /// <summary>
        /// Retrieve customer's rewards profile.
        /// </summary>
        /// <param name="customerId">Loyalty customer ID</param>
        /// <param name="preferredDataSource">Indicates which data source to retrieve data from.</param>
        /// <returns>CustomerRewardsProfile object which contains customer's rewards information.</returns>
        Task<CustomerRewardsProfile> GetRewardsInfoAsync(Guid customerId, PreferredDataSource preferredDataSource);
    }
}