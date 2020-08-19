namespace SampleRewardsLookupService.Models
{
    using System;

    public class Note
    {
        /// <summary>
        /// Note ID
        /// </summary>
        public string NoteId { get; set; }

        /// <summary>
        /// Access code
        /// </summary>
        public string AccessCode { get; set; }

        /// <summary>
        /// Initial value of the note.
        /// </summary>
        public decimal InitialValue { get; set; }

        /// <summary>
        /// Current value of the note.
        /// </summary>
        public decimal CurrentValue { get; set; }

        /// <summary>
        /// Expiration date/time of the note.
        /// </summary>
        public DateTimeOffset ExpirationDate { get; set; }
    }
}