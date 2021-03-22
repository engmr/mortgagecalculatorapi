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
        private IAuthTokenProvider _authTokenProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="errorMessagesLocalizer"></param>
        /// <param name="validationMessagesLocalizer"></param>
        /// <param name="authTokenProvider"></param>
        /// <param name="appSettings"></param>
        public AuthorizeController(ILoggerFactory loggerFactory,
            IStringLocalizer<ErrorMessages> errorMessagesLocalizer,
            IStringLocalizer<ValidationMessages> validationMessagesLocalizer,
            IAuthTokenProvider authTokenProvider,
            IOptions<AppSettings> appSettings)
            : base(loggerFactory, authTokenProvider, appSettings)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<AuthorizeController>();
            _errorMessagesLocalizer = errorMessagesLocalizer ?? throw new ArgumentNullException(nameof(errorMessagesLocalizer));
            _validationMessagesLocalizer = validationMessagesLocalizer ?? throw new ArgumentNullException(nameof(validationMessagesLocalizer));
            _authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
        }

        /// <summary>
        /// Calculate (free)
        /// </summary>
        /// <param name="authHeaders"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("issuetoken")]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> IssueToken([FromHeader] AuthorizationHeaders authHeaders)
        {
            using (_logger.BeginScope(GetTransactionLoggingScope()))
            {
                _logger.LogDebug($"{nameof(IssueToken)}" + " called");
                if (!CheckAuthorizationHeadersForCredentials(authHeaders))
                {
                    return GetFailedAuthorizationResponse();
                }

                return await Task.Run(() =>
                {
                    var issuedToken = _authTokenProvider.VerifyIsPaidUserAndIssueToken(authHeaders.ClientId, authHeaders.Password, out string token);
                    if (issuedToken)
                    {
                        return Ok(GetApiResponse(token));
                    }
                    return GetFailedAuthorizationResponse();
                });
            }
        }
    }
}
