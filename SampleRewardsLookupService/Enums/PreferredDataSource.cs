namespace SampleRewardsLookupService.Enums
{
    /// <summary>
    /// Enum to indicate which data source to retrieve data from.
    /// </summary>
    public enum PreferredDataSource
    {
        /// <summary>
        /// Use vendor's live data.
        /// </summary>
        VendorXLiveData,

        /// <summary>
        /// Use cached data stored in Loyalty domain.
        /// </summary>
        RedisCacheData
    }
}