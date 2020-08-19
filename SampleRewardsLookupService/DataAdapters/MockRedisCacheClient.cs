namespace SampleRewardsLookupService.DataAdapters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SampleRewardsLookupService.Models;

    /// <summary>
    /// A fake Redis Client to simulate retrieving data from Loyalty's Redis cache.
    /// </summary>
    public class MockRedisCacheClient : IRedisCacheClient
    {
        /// <summary>
        /// Retrieve Rewards information from cache.
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>CustomerRewardsProfile object which contains customer's rewards information.</returns>
        public async Task<CustomerRewardsProfile> GetRewardsDataAsync(Guid customerId)
        {
            //CREATING A RANDOM CustomerRewardsProfile OBJECT ADD ADDED ARTIFICIAL DELAY TO SIMULATE DATA RETRIVAL FROM CACHE
            //const int RANDOM_SEED = 0;
            //var random = new Random(RANDOM_SEED);
            var random = new Random();

            //SIMULATE 10% CACHE MISS
            if (random.Next(100) % 10 == 0)
            {
                return null;
            }

            var randomPointEarned = random.Next(2000);

            await Task.Delay(random.Next(2, 25)).ConfigureAwait(false);

            return new CustomerRewardsProfile()
            {
                LevelInformation = new RewardsLevel()
                {
                    Level = random.Next(5),
                    PointsEarnedYTD = randomPointEarned,
                    PointsEarnedLifetime = randomPointEarned * random.Next(1, 10)
                },
                NotesInformation = MockGenerateNotes(random.Next(5))
            };
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