using MAR.API.MortgageCalculator.Localization;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Model.Dto;
using MAR.API.MortgageCalculator.Model.Requests;
using MAR.API.MortgageCalculator.Model.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MAR.API.MortgageCalculator.Controllers
{
    /// <summary>
    /// Calculation controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CalculateController : ControllerBase
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
            var transactionId = GetTransactionId();
            return await Task.Run<IActionResult>(() =>
            {
                var result = _mortgageCalculatorFacade.GetMortgageCalculation(request);
                if (result.IsSuccessful)
                {
                    return Ok(new ApiResponse<MortgageCalculationResult>()
                    {
                        ResponseDateTime = DateTime.UtcNow,
                        APIVersion = GetApiVersion(),
                        ApplicationName = GetApplicationName(),
                        TransactionId = transactionId,
                        Data = result as MortgageCalculationResult
                    });
                }
                return StatusCode((int)HttpStatusCode.BadRequest, new ApiResponse<MortgageCalculationResult>()
                {
                    ResponseDateTime = DateTime.UtcNow,
                    APIVersion = GetApiVersion(),
                    ApplicationName = GetApplicationName(),
                    TransactionId = transactionId,
                    Data = result as MortgageCalculationResult
                });
            });
        }

        /// <summary>
        /// Calculate (secure)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> CalculateSecure([FromBody] MortgageCalculationRequest request)
        {
            var transactionId = GetTransactionId();
            return await Task.Run<IActionResult>(() =>
            {
                var result = _mortgageCalculatorFacade.GetMortgageCalculation(request);
                if (result.IsSuccessful)
                {
                    return Ok(new ApiResponse<MortgageCalculationResult>()
                    {
                        ResponseDateTime = DateTime.UtcNow,
                        APIVersion = GetApiVersion(),
                        ApplicationName = GetApplicationName(),
                        TransactionId = transactionId,
                        Data = result as MortgageCalculationResult
                    });
                }
                return StatusCode((int)HttpStatusCode.BadRequest, new ApiResponse<MortgageCalculationResult>()
                {
                    ResponseDateTime = DateTime.UtcNow,
                    APIVersion = GetApiVersion(),
                    ApplicationName = GetApplicationName(),
                    TransactionId = transactionId,
                    Data = result as MortgageCalculationResult
                });
            });
        }

        #region Move to base class / implement later

        private Guid GetTransactionId()
        {
            return Guid.NewGuid();
        }

        private string GetApiVersion()
        {
            //return _apiVersionReader.Read(HttpContext.Request);
            return "1.0.1";
        }

        private string GetApplicationName()
        {
            //return _appSettings.ApplicationName;
            return "Mortgage Calculator API";
        }

        #endregion
    }
}
