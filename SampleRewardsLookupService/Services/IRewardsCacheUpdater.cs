namespace SampleRewardsLookupService.Services
{
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Models;

    public interface IRewardsCacheUpdater
    {
        Task<bool> UpdateRewardsInfoAsync(CustomerRewardsProfile customerId);
    }
}