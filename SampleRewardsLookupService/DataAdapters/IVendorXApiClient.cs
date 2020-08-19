namespace SampleRewardsLookupService.DataAdapters
{
    using System;
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Models;

    /// <summary>
    /// HTTP Client for calling Vendor's API
    /// </summary>
    public interface IVendorXApiClient
    {
        /// <summary>
        /// Retrieve call vendor's API to translate Loyalty's customer ID to vendor's customer ID.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>Vendor's customer ID</returns>
        Task<Guid> GetVendorCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Retrieve vendor's Rewards information via a HTTPs Request
        /// </summary>
        /// <param name="vendorCustomerId">Customer ID used in vendor's system.</param>
        /// <returns>VendorXRewardsData object that is deserialized from vendor's HTTP response.</returns>
        Task<VendorXRewardsData> GetRewardsDataAsync(Guid vendorCustomerId);
    }
}