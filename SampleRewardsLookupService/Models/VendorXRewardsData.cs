namespace SampleRewardsLookupService.Models
{
    /// <summary>
    /// Vendor's rewards data model
    /// </summary>
    public class VendorXRewardsData
    {
        /// <summary>
        /// Loyalty level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Points earned this year.
        /// </summary>
        public int PointsEarnedYTD { get; set; }

        /// <summary>
        /// Points earned since account was created.
        /// </summary>
        public int PointsEarnedLifetime { get; set; }

        /// <summary>
        /// Customer's notes
        /// </summary>
        public Notes Notes { get; set; }
    }
}