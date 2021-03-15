using System;

namespace MAR.API.MortgageCalculator.Model.Dto
{
    public class ApiResponse<T> where T : class
    {
        /// <summary>
        /// UTC
        /// </summary>
        public DateTime ResponseDateTime { get; set; }
        /// <summary>
        /// Major Minor Build (e.g. 1.2.55)
        /// </summary>
        public string APIVersion { get; set; }
        /// <summary>
        /// User friendly application name (e.g. Weather Forecast API)
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Unique id for logging, etc.
        /// </summary>
        public Guid TransactionId { get; set; }
        public T Data { get; set; }
    }
}
