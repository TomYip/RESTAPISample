namespace SampleRewardsLookupService.DataAdapters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Models;

    /// <summary>
    /// Mock Client to simulate HTTP calls to Vendor's API
    /// </summary>
    public class MockVendorXApiClient : IVendorXApiClient
    {
        /// <summary>
        /// Simulate a call to vendor's API to translate Loyalty's customer ID to vendor's customer ID.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>Vendor's customer ID</returns>
        public async Task<Guid> GetVendorCustomerIdAsync(Guid customerId)
        {
            // TODO: PRETENDING CUSTOMER ID IS THE SAME AS VENDOR'S CUSTOMER ID
            return await Task.FromResult(customerId).ConfigureAwait(false);
        }

        /// <summary>
        /// Simulate retrieving vendor's Rewards information via a HTTPs Request
        /// </summary>
        /// <param name="vendorCustomerId">Customer ID used in vendor's system.</param>
        /// <returns>VendorXRewardsData object that is deserialized from vendor's HTTP response.</returns>
        /// <remarks>TODO: MAKE REAL CALLS TO VENDER'S API TO RETRIEVE CUSTOMER'S DATA</remarks>
        public async Task<VendorXRewardsData> GetRewardsDataAsync(Guid vendorCustomerId)
        {
            //[MOCK] CREATING A RANDOM VendorXRewardsData OBJECT ADD ADDED ARTIFICIAL DELAY TO SIMULATE A RESPONSE FROM VENDOR

            //const int RANDOM_SEED = 0;
            //var random = new Random(RANDOM_SEED);
            var random = new Random();

            var randomPointEarned = random.Next(2000);
            var response = new VendorXRewardsData()
            {
                Level = random.Next(5),
                PointsEarnedYTD = randomPointEarned,
                PointsEarnedLifetime = randomPointEarned * random.Next(1, 10),
                Notes = MockGenerateNotes(random.Next(5))
            };

            await Task.Delay(random.Next(300, 1500)).ConfigureAwait(false);

            return response;
        }

        private Notes MockGenerateNotes(int count, int? randomSeed = null)
        {
            var notes = new List<Note>();
            for (int i = 0; i < count; i++)
            {
                var newNote = MockGenerateANote(randomSeed);
                notes.Add(newNote);
            }

            return new Notes()
            {
                MyNotes = notes.ToArray()
            };
        }

        private Note MockGenerateANote(int? randomSeed = null)
        {
            Random random = randomSeed is null ? new Random() : new Random((int)randomSeed);
            var initValue = random.Next(100);

            return new Note()
            {
                NoteId = random.Next(1000000000).ToString("###-###-###"),
                AccessCode = random.Next(1000000).ToString(),
                InitialValue = initValue,
                CurrentValue = Math.Max(0, initValue - random.Next(initValue) * 1.2M),
                ExpirationDate = new DateTimeOffset(2019 + random.Next(3), random.Next(1, 13), random.Next(1, 29), 0, 0, 0, new TimeSpan(0L)),
            };
        }
    }
}