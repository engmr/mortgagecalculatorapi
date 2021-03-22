using System;

namespace MAR.API.MortgageCalculator
{
    /// <summary>
    /// Application settings for the API
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// User friendly application name
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Public paid access user id for testing
        /// </summary>
        public Guid PublicPaidAccessUserId { get; set; }
        /// <summary>
        /// Public paid access user password for testing
        /// </summary>
        public string PublicPaidAccessUserPassword { get; set; }
    }
}
