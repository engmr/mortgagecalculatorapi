using MAR.API.MortgageCalculator.Interfaces;
using MAR.API.MortgageCalculator.Localization;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Model.Requests;
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
    /// Calculation controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CalculateController : DomainBaseController
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<CalculateController> _logger;
        private IStringLocalizer<ErrorMessages> _errorMessagesLocalizer;
        private IStringLocalizer<ValidationMessages> _validationMessagesLocalizer;
        private IMortgageCalculatorFacade _mortgageCalculatorFacade;
        private IAuthorizationProvider _authorizationProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="errorMessagesLocalizer"></param>
        /// <param name="validationMessagesLocalizer"></param>
        /// <param name="mortgageCalculatorFacade"></param>
        /// <param name="authorizationProvider"></param>
        /// <param name="appSettings"></param>
        public CalculateController(ILoggerFactory loggerFactory,
            IStringLocalizer<ErrorMessages> errorMessagesLocalizer,
            IStringLocalizer<ValidationMessages> validationMessagesLocalizer,
            IMortgageCalculatorFacade mortgageCalculatorFacade,
            IAuthorizationProvider authorizationProvider,
            IOptions<AppSettings> appSettings)
            : base(loggerFactory, authorizationProvider, appSettings)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<CalculateController>();
            _errorMessagesLocalizer = errorMessagesLocalizer ?? throw new ArgumentNullException(nameof(errorMessagesLocalizer));
            _validationMessagesLocalizer = validationMessagesLocalizer ?? throw new ArgumentNullException(nameof(validationMessagesLocalizer));
            _mortgageCalculatorFacade = mortgageCalculatorFacade ?? throw new ArgumentNullException(nameof(mortgageCalculatorFacade));
            _authorizationProvider = authorizationProvider ?? throw new ArgumentNullException(nameof(authorizationProvider));
        }

        /// <summary>
        /// Calculate (free)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("free")]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> Calculate([FromBody] MortgageCalculationRequest request)
        {
            using (_logger.BeginScope(GetTransactionLoggingScope()))
            {
                _logger.LogInformation($"{nameof(Calculate)}" + " called with request {@request}");
                return await Task.Run(() =>
                {
                    var result = _mortgageCalculatorFacade.GetMortgageCalculation(request);
                    return GetResultForMortageCalculation(result);
                });
            }
        }

        /// <summary>
        /// Calculate (paid)
        /// </summary>
        /// <param name="authHeaders"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> CalculatePaid([FromHeader] AuthorizationHeaders authHeaders, [FromBody] MortgageCalculationRequest request)
        {
            using (_logger.BeginScope(GetTransactionLoggingScope()))
            {
                _logger.LogInformation($"{nameof(CalculatePaid)}" + " called with request {@request}");
                if (!CheckAuthorizationHeadersForClientAndToken(authHeaders))
                {
                    return GetBadRequestResponse();
                }

                if (_authorizationProvider.IsTokenStillValid(authHeaders.ClientId, authHeaders.AuthorizationToken))
                {
                    return await Task.Run(() =>
                    {
                        var result = _mortgageCalculatorFacade.GetMortgageCalculation(request);
                        return GetResultForMortageCalculation(result);
                    });
                }

                return GetUnauthorizedResponse();
            }
        }

        /// <summary>
        /// Calculate (bulk)
        /// </summary>
        /// <param name="authHeaders"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [Produces("application/json")]
        [Route("bulk")]
        [HttpPost]
        public async Task<IActionResult> CalculateBulk([FromHeader] AuthorizationHeaders authHeaders, [FromBody] BulkMortgageCalculationRequest request)
        {
            using (_logger.BeginScope(GetTransactionLoggingScope()))
            {
                _logger.LogInformation($"{nameof(CalculatePaid)}" + " called with request {@request}");
                if (!CheckAuthorizationHeadersForClientAndToken(authHeaders))
                {
                    return GetBadRequestResponse();
                }

                if (_authorizationProvider.IsTokenStillValid(authHeaders.ClientId, authHeaders.AuthorizationToken))
                {
                    return await Task.Run(() =>
                    {
                        var result = _mortgageCalculatorFacade.GetMortgageCalculations(request);
                        return GetResultForMortageCalculation(result);
                    });
                }

                return GetUnauthorizedResponse();
            }
        }
    }
}
