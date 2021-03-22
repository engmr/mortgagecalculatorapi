using MAR.API.MortgageCalculator.Interfaces;
using MAR.API.MortgageCalculator.Model.Dto;
using MAR.API.MortgageCalculator.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MAR.API.MortgageCalculator.Controllers
{
    /// <summary>
    /// Shared logic amongst domain API controllers
    /// </summary>
    public class DomainBaseController : ControllerBase
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<DomainBaseController> _logger;
        private IAuthTokenProvider _authTokenProvider;
        private IOptions<AppSettings> _appSettings;

        private Guid _transactionId;
        /// <summary>
        /// Domain API transaction id
        /// </summary>
        protected Guid TransactionId => _transactionId;

        private const string TransactionIdKey = "TransactionId";

        /// <summary>
        /// Constructor
        /// </summary>
        public DomainBaseController(ILoggerFactory loggerFactory, IAuthTokenProvider authTokenProvider, IOptions<AppSettings> appSettings)
        {
            SetTransactionId();
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<DomainBaseController>();
            _authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        /// <summary>
        /// Checks auth headers to see if credentials were provided
        /// </summary>
        /// <param name="authHeaders"></param>
        /// <returns>true if credentials are present</returns>
        protected bool CheckAuthorizationHeadersForCredentials(AuthorizationHeaders authHeaders)
        {
            if (authHeaders == null)
            {
                _logger.LogError("No authorization headers received");
                return false;
            }

            if (string.IsNullOrWhiteSpace(authHeaders.ClientId)
                || string.IsNullOrWhiteSpace(authHeaders.Password))
            {
                _logger.LogError("No authorization token received");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks auth headers to see if auth token was provided
        /// </summary>
        /// <param name="authHeaders"></param>
        /// <returns>true if token is present</returns>
        protected bool CheckAuthorizationHeadersForClientAndToken(AuthorizationHeaders authHeaders)
        {
            if (authHeaders == null)
            {
                _logger.LogError("No authorization headers received");
                return false;
            }

            if (string.IsNullOrWhiteSpace(authHeaders.AuthorizationToken))
            {
                _logger.LogError("No authorization token received");
                return false;
            }

            if (string.IsNullOrWhiteSpace(authHeaders.ClientId))
            {
                _logger.LogError("No client id received");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get ApiResponse object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ApiResponse<object> GetApiResponse(object data)
        {
            return new ApiResponse<object>()
            {
                ResponseDateTime = DateTime.UtcNow,
                APIVersion = GetApiVersion(),
                ApplicationName = GetApplicationName(),
                TransactionId = TransactionId,
                Data = data
            };
        }

        /// <summary>
        /// Gets general failed auth result
        /// </summary>
        /// <returns></returns>
        protected IActionResult GetFailedAuthorizationResponse()
        {
            return StatusCode((int)HttpStatusCode.BadRequest, GetApiResponse(null));
        }

        /// <summary>
        /// Gets unauthorized result
        /// </summary>
        /// <returns></returns>
        protected IActionResult GetUnauthorizedResponse()
        {
            return StatusCode((int)HttpStatusCode.Unauthorized, GetApiResponse(null));
        }

        /// <summary>
        /// Get ActionResult from a mortgage calculation result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult GetResultForMortageCalculation(IMortgageCalculationResult result)
        {
            if (result.IsSuccessful)
            {
                return Ok(GetApiResponse(result));
            }
            if (result.ValidationErrors?.Any() == true)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, GetApiResponse(result));
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, GetApiResponse(result));
        }

        /// <summary>
        /// Get transction level logging scope
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, object> GetTransactionLoggingScope()
        {
            return new Dictionary<string, object>()
            {
                { TransactionIdKey, TransactionId }
            };
        }

        /// <summary>
        /// Gets API's version
        /// </summary>
        /// <returns></returns>
        protected string GetApiVersion()
        {
            //return _apiVersionReader.Read(HttpContext.Request);
            return "1.0.3";
        }

        /// <summary>
        /// Gets API user friendly application name
        /// </summary>
        /// <returns></returns>
        protected string GetApplicationName()
        {
            return _appSettings.Value.ApplicationName;
        }

        /// <summary>
        /// Gets API request transaction Id
        /// </summary>
        /// <returns></returns>
        protected void SetTransactionId()
        {
            if (_transactionId == Guid.Empty)
            {
                _transactionId = Guid.NewGuid();
            }
        }
    }
}
