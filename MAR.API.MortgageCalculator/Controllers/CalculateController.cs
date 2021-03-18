using MAR.API.MortgageCalculator.Localization;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Model.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="errorMessagesLocalizer"></param>
        /// <param name="validationMessagesLocalizer"></param>
        /// <param name="mortgageCalculatorFacade"></param>
        public CalculateController(ILoggerFactory loggerFactory, 
            IStringLocalizer<ErrorMessages> errorMessagesLocalizer,
            IStringLocalizer<ValidationMessages> validationMessagesLocalizer, 
            IMortgageCalculatorFacade mortgageCalculatorFacade)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<CalculateController>();
            _errorMessagesLocalizer = errorMessagesLocalizer ?? throw new ArgumentNullException(nameof(errorMessagesLocalizer));
            _validationMessagesLocalizer = validationMessagesLocalizer ?? throw new ArgumentNullException(nameof(validationMessagesLocalizer));
            _mortgageCalculatorFacade = mortgageCalculatorFacade ?? throw new ArgumentNullException(nameof(mortgageCalculatorFacade));
        }

        /// <summary>
        /// Calculate (free)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// Add rate limiting to this later
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
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> CalculatePaid([FromBody] MortgageCalculationRequest request)
        {
            using (_logger.BeginScope(GetTransactionLoggingScope()))
            {
                _logger.LogInformation($"{nameof(CalculatePaid)}" + " called with request {@request}");
                return await Task.Run(() =>
                {
                    var result = _mortgageCalculatorFacade.GetMortgageCalculation(request);
                    return GetResultForMortageCalculation(result);
                });
            }
        }
    }
}
