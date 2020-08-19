namespace SampleRewardsLookupServiceUnitTests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using SampleRewardsLookupService.Constants;
    using SampleRewardsLookupService.Enums;
    using SampleRewardsLookupService.Models;
    using SampleRewardsLookupService.Services;

    public class RewardsLookupOrchestratorUnitTests
    {
        private RewardsLookupOrchestrator orchestrator;
        private Guid randomCustomerId;

        private Mock<IRewardsInfoRetriever> mockLiveDataSource;
        private Mock<IRewardsInfoRetriever> mockCacheDataSource;
        private Mock<IRewardsCacheUpdater> mockCacheUpdater;
        private Mock<ILogger<RewardsLookupOrchestrator>> mockLogger;
        private Mock<IConfiguration> mockConfig;

        [SetUp]
        public void Setup()
        {
            this.mockLiveDataSource = new Mock<IRewardsInfoRetriever>();
            this.mockCacheDataSource = new Mock<IRewardsInfoRetriever>();
            this.mockCacheUpdater = new Mock<IRewardsCacheUpdater>();
            this.mockLogger = new Mock<ILogger<RewardsLookupOrchestrator>>();
            this.mockConfig = new Mock<IConfiguration>();

            // Minimal setup to return some mock data from a mock object
            this.mockLiveDataSource.Setup(m => m.GetRewardsInfoAsync(It.IsAny<Guid>()))
                                                .ReturnsAsync(new CustomerRewardsProfile()
                                                {
                                                    DataSource = LoyaltyConstants.VENDOR_LIVE_DATA
                                                });

            // Minimal setup to return some mock data from a mock object
            this.mockCacheDataSource.Setup(m => m.GetRewardsInfoAsync(It.IsAny<Guid>()))
                                                .ReturnsAsync(new CustomerRewardsProfile()
                                                {
                                                    DataSource = LoyaltyConstants.REWARDS_CACHE
                                                });

            orchestrator = new RewardsLookupOrchestrator(
                mockLiveDataSource.Object,
                mockCacheDataSource.Object,
                mockCacheUpdater.Object,
                mockLogger.Object,
                mockConfig.Object);

            randomCustomerId = Guid.NewGuid();
        }

        [Test]
        public void DefaultConstructorWorks()
        {
            Assert.IsNotNull(orchestrator);
        }

        [Test]
        public async Task RetrieveDataByDefaultGetsDataFromLiveSource()
        {
            var result = await orchestrator.GetRewardsInfoAsync(randomCustomerId).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(LoyaltyConstants.VENDOR_LIVE_DATA, result.DataSource);
        }

        [Test]
        public async Task RetrieveDataFromLiveDataSourceGetsDataFromLiveSource()
        {
            var result = await orchestrator.GetRewardsInfoAsync(randomCustomerId, PreferredDataSource.VendorXLiveData).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(LoyaltyConstants.VENDOR_LIVE_DATA, result.DataSource);
        }

        [Test]
        public async Task RetrieveDataFromLiveDataSourceWithFallbackGetsDataFromCacheSource()
        {
            //Simulate timeout exception which triggers fallback
            this.mockLiveDataSource.Setup(m => m.GetRewardsInfoAsync(It.IsAny<Guid>()))
                                                .ThrowsAsync(new TimeoutException());

            var result = await orchestrator.GetRewardsInfoAsync(randomCustomerId, PreferredDataSource.VendorXLiveData).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(LoyaltyConstants.REWARDS_CACHE, result.DataSource);
        }

        [Test]
        public async Task RetrieveDataFromCacheDataSourceGetsDataFromCacheSource()
        {
            var result = await orchestrator.GetRewardsInfoAsync(randomCustomerId, PreferredDataSource.RedisCacheData).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(LoyaltyConstants.REWARDS_CACHE, result.DataSource);
        }

        [Test]
        public async Task RetrieveDataFromCacheDataSourceWithFallbackGetsDataFromLiveSource()
        {
            //Simulate cache miss which triggers fallback
            this.mockCacheDataSource.Setup(m => m.GetRewardsInfoAsync(It.IsAny<Guid>()))
                                                 .Returns(Task.FromResult<CustomerRewardsProfile>(null));

            var result = await orchestrator.GetRewardsInfoAsync(randomCustomerId, PreferredDataSource.RedisCacheData).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(LoyaltyConstants.VENDOR_LIVE_DATA, result.DataSource);
        }
    }
}