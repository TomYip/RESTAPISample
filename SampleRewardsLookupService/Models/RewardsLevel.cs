namespace SampleRewardsLookupService.Models
{
    /// <summary>
    /// Customer's rewards level information.
    /// </summary>
    public class RewardsLevel
    {
        /// <summary>
        /// Current level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Points earned since this year.
        /// </summary>
        public int PointsEarnedYTD { get; set; }

        /// <summary>
        /// Points earned since Loyalty account was created.
        /// </summary>
        public int PointsEarnedLifetime { get; set; }
    }
}