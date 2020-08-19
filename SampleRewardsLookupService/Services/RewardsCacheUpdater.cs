namespace SampleRewardsLookupService.Services
{
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Models;

    public class RewardsCacheUpdater : IRewardsCacheUpdater
    {
        public Task<bool> UpdateRewardsInfoAsync(CustomerRewardsProfile customerId)
        {
            //TODO: IMPLEMENT THE REAL CACHE UPDATE LOGIC
            return Task.FromResult(true);
        }
    }
}