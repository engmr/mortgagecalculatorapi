using MAR.API.MortgageCalculator.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace MAR.API.MortgageCalculator.Providers
{
    /// <summary>
    /// Authorization token provider. This type of implementation only works for distributed systems if using distributed mem caching.
    /// Normally, you would want a dedicated microservice to handle authorization tokens and expiration.
    /// </summary>
    public class AuthTokenProvider : IAuthTokenProvider
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<AuthTokenProvider> _logger;
        private IMemoryCache _memoryCache;
        private IOptions<AppSettings> _appSettings;

        private const string _authToken_ClientId_CacheKeyFormat = "AUTHTOKEN_CLIENTID_{0}";
        private const string _paidUsers_ClientId_CacheKeyFormat = "PAIDUSERS_CLIENTID_{0}";
        private const string _paidUsers_ClientId_Password_CacheKeyFormat = "PAIDUSERS_CLIENTID_{0}_PASSWORD";
        private const int _authToken_ClientId_Timeout = 3600000; //1 hour
        private MemoryCacheEntryOptions _authToken_ClientId_MemoryCacheEntryOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="memoryCache"></param>
        /// <param name="appSettings"></param>
        public AuthTokenProvider(ILoggerFactory loggerFactory, IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<AuthTokenProvider>();
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            _authToken_ClientId_MemoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMilliseconds(_authToken_ClientId_Timeout));
        }

        /// <summary>
        /// Expires authorization token
        /// </summary>
        /// <returns>true if successful</returns>
        public bool ExpireAuthTokenForClientId(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                _logger.LogError($"{nameof(clientId)} was not provided.");
                return false;
            }
            var cacheKey = GetAuthTokenCacheKeyForClientId(clientId);
            if (_memoryCache.TryGetValue(cacheKey, out var cacheValue))
            {
                _memoryCache.Remove(cacheKey);
                _logger.LogInformation($"ClientId '{clientId}' authorization token expiration performed.");
            }
            else
            {
                _logger.LogDebug($"ClientId '{clientId}' authorization token was already expired.");
            }
            
            return true;
        }

        /// <summary>
        /// Gets authorization token
        /// </summary>
        /// <returns>null if token is not found</returns>
        public string GetAuthTokenForClientId(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                _logger.LogError($"{nameof(clientId)} was not provided.");
                return null;
            }
            var cacheKey = GetAuthTokenCacheKeyForClientId(clientId);
            if (_memoryCache.TryGetValue(cacheKey, out string token))
            {
                _logger.LogDebug($"ClientId '{clientId}' authorization token retrieved.");
                return token;
            }
            else
            {
                _logger.LogDebug($"ClientId '{clientId}' authorization token was not found.");
                return null;
            }
        }

        /// <summary>
        /// Checks if token is still valid for client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsTokenStillValid(string clientId, string token)
        {
            if (string.IsNullOrWhiteSpace(clientId)
                || string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError($"{nameof(clientId)} or {nameof(token)} was not provided.");
                return false;
            }

            var existingClientAuthToken = GetAuthTokenForClientId(clientId);
            return string.Equals(existingClientAuthToken, token, StringComparison.Ordinal);
        }

        /// <summary>
        /// Checks identity and issues token if successful. If token is already present, returns existing token.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="password"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool VerifyIsPaidUserAndIssueToken(string clientId, string password, out string token)
        {
            if (string.IsNullOrWhiteSpace(clientId) 
                || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError($"{nameof(clientId)} or {nameof(password)} was not provided.");
                token = null;
                return false;
            }

            var paidUserClientCacheKey = GetPaidUserCacheKeyForClientId(clientId);
            var paidUserPasswordCacheKey = GetPaidUserPasswordCacheKeyForClientId(clientId);

            if (_memoryCache.TryGetValue(paidUserClientCacheKey, out Guid cachedClientId)
                && _memoryCache.TryGetValue(paidUserPasswordCacheKey, out string cachedClientPassword))
            {
                if (Guid.TryParse(clientId, out Guid inputClientId)
                    && cachedClientId == inputClientId
                    && string.Equals(cachedClientPassword, password))
                {
                    var clientExistingAuthToken = GetAuthTokenForClientId(clientId);
                    //Issue new token
                    if (clientExistingAuthToken == null)
                    {
                        var newToken = string.Empty;
                        UpsertAuthTokenForClientId(clientId, ref newToken);
                        token = newToken;
                        return true;
                    }
                    token = clientExistingAuthToken;
                    return true; 
                }
            }

            token = null;
            return false;
        }

        /// <summary>
        /// Inserts authorization token. If already there, extends the expiration time.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="token"></param>
        /// <returns>true if successful</returns>
        public bool UpsertAuthTokenForClientId(string clientId, ref string token)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                _logger.LogError($"{nameof(clientId)} was not provided.");
                return false;
            }

            var cacheKey = GetAuthTokenCacheKeyForClientId(clientId);

            if (_memoryCache.TryGetValue(cacheKey, out token))
            {
                _logger.LogDebug($"Extending expiration for ClientId '{clientId}' authorization token.");
                _memoryCache.Set(cacheKey, token, _authToken_ClientId_MemoryCacheEntryOptions);
                return true;
            }
            _logger.LogDebug($"Issuing new authorization token for ClientId '{clientId}'.");
            token = Guid.NewGuid().ToString();
            _memoryCache.Set(cacheKey, token, _authToken_ClientId_MemoryCacheEntryOptions);
            return true;
        }

        private string GetAuthTokenCacheKeyForClientId(string clientId)
        {
            return string.Format(_authToken_ClientId_CacheKeyFormat, clientId);
        }

        private string GetPaidUserCacheKeyForClientId(string clientId)
        {
            return string.Format(_paidUsers_ClientId_CacheKeyFormat, clientId);
        }

        private string GetPaidUserPasswordCacheKeyForClientId(string clientId)
        {
            return string.Format(_paidUsers_ClientId_Password_CacheKeyFormat, clientId);
        }
    }
}
