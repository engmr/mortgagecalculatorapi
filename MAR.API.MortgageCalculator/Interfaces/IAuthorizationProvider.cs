namespace MAR.API.MortgageCalculator.Interfaces
{
    /// <summary>
    /// Authorization token provider interface
    /// </summary>
    public interface IAuthorizationProvider
    {
        /// <summary>
        /// Expires authorization token
        /// </summary>
        /// <returns>true if successful</returns>
        bool ExpireAuthTokenForClientId(string clientId);
        /// <summary>
        /// Gets authorization token
        /// </summary>
        /// <returns>null if token is not found</returns>
        string GetAuthTokenForClientId(string clientId);
        /// <summary>
        /// Checks if client's token is still valid
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsTokenStillValid(string clientId, string token);
        /// <summary>
        /// Inserts or updates authorization token
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="token"></param>
        /// <returns>true if successful</returns>
        bool UpsertAuthTokenForClientId(string clientId, ref string token);
        /// <summary>
        /// Checks identity and issues token (if valid user)
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="password"></param>
        /// <param name="token"></param>
        /// <returns>true if successful</returns>
        bool VerifyIsPaidUserAndIssueToken(string clientId, string password, out string token);
    }
}
