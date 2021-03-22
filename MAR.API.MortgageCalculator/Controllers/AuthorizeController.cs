using MAR.API.MortgageCalculator.Interfaces;
using MAR.API.MortgageCalculator.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace MAR.API.MortgageCalculator.Controllers
{
    /// <summary>
    /// Authorization controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthorizeController : DomainBaseController
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<AuthorizeController> _logger;
        private IStringLocalizer<ErrorMessages> _errorMessagesLocalizer;
        private IStringLocalizer<ValidationMessages> _validationMessagesLocalizer;
        private IAuthorizationProvider _authorizationProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="errorMessagesLocalizer"></param>
        /// <param name="validationMessagesLocalizer"></param>
        /// <param name="authorizationProvider"></param>
        /// <param name="appSettings"></param>
        public AuthorizeController(ILoggerFactory loggerFactory,
            IStringLocalizer<ErrorMessages> errorMessagesLocalizer,
            IStringLocalizer<ValidationMessages> validationMessagesLocalizer,
            IAuthorizationProvider authorizationProvider,
            IOptions<AppSettings> appSettings)
            : base(loggerFactory, authorizationProvider, appSettings)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<AuthorizeController>();
            _errorMessagesLocalizer = errorMessagesLocalizer ?? throw new ArgumentNullException(nameof(errorMessagesLocalizer));
            _validationMessagesLocalizer = validationMessagesLocalizer ?? throw new ArgumentNullException(nameof(validationMessagesLocalizer));
            _authorizationProvider = authorizationProvider ?? throw new ArgumentNullException(nameof(authorizationProvider));
        }

        /// <summary>
        /// Issue token
        /// </summary>
        /// <param name="authHeaders"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("token/issue")]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> IssueToken([FromHeader] AuthorizationHeaders authHeaders)
        {
            using (_logger.BeginScope(GetTransactionLoggingScope()))
            {
                _logger.LogDebug($"{nameof(IssueToken)}" + " called");
                if (!CheckAuthorizationHeadersForCredentials(authHeaders))
                {
                    return GetBadRequestResponse();
                }

                return await Task.Run(() =>
                {
                    var issuedToken = _authorizationProvider.VerifyIsPaidUserAndIssueToken(authHeaders.ClientId, authHeaders.Password, out string token);
                    if (issuedToken)
                    {
                        return Ok(GetApiResponse(token));
                    }
                    return GetUnauthorizedResponse();
                });
            }
        }

        /// <summary>
        /// Check if token is still valid
        /// </summary>
        /// <param name="authHeaders"></param>
        /// <returns></returns>
        [Route("token/isvalid")]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> IsTokenStillValid([FromHeader] AuthorizationHeaders authHeaders)
        {
            using (_logger.BeginScope(GetTransactionLoggingScope()))
            {
                _logger.LogDebug($"{nameof(IsTokenStillValid)}" + " called");
                if (!CheckAuthorizationHeadersForClientAndToken(authHeaders))
                {
                    return GetBadRequestResponse();
                }

                return await Task.Run(() =>
                {
                    var tokenIsStillValid = _authorizationProvider.IsTokenStillValid(authHeaders.ClientId, authHeaders.AuthorizationToken);
                    return Ok(GetApiResponse(tokenIsStillValid));
                });
            }
        }
    }
}
