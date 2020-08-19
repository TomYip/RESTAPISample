namespace SampleRewardsLookupService.Utilities
{
    using System;

    public static class ObjectExtensions
    {
        /// <summary>
        /// (Extension) A useful method to verify object is not null before it is assigned to another variable.
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="source">Object class for the extension method.</param>
        /// <param name="name">Name of the objec to check for null.</param>
        /// <returns></returns>
        public static T ThrowIfNull<T>(this T source, string name)
        {
            if (source == null)
            {
                throw new ArgumentNullException(name);
            }

            return source;
        }
    }
}