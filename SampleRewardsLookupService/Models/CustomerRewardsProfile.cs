namespace SampleRewardsLookupService.Models
{
    using System;

    /// <summary>
    /// Model object that wraps customer's rewards information.
    /// </summary>
    public class CustomerRewardsProfile
    {
        /// <summary>
        /// Customer ID - a GUID
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Contains notes related information.
        /// </summary>
        public Notes NotesInformation { get; set; }

        /// <summary>
        /// Contains information related to customer's rewards level.
        /// </summary>
        public RewardsLevel LevelInformation { get; set; }

        /// <summary>
        /// Indicates which data source the inforamtion is retrieved from.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Operation metrics to show how much time it takes to retrieve data from data source.
        /// </summary>
        public int RetrivalTime { get; set; }
    }
}