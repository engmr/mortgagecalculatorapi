using Microsoft.AspNetCore.Mvc;

namespace MAR.API.MortgageCalculator
{
    /// <summary>
    /// For authorization requests
    /// </summary>
    public class AuthorizationHeaders
    {
        /// <summary>
        /// Client id
        /// </summary>
        [FromHeader]
        public string ClientId { get; set; }
        /// <summary>
        /// Password for client id
        /// </summary>
        [FromHeader]
        public string Password { get; set; }
        /// <summary>
        /// Authorization token
        /// </summary>
        [FromHeader]
        public string AuthorizationToken { get; set; }
    }
}
