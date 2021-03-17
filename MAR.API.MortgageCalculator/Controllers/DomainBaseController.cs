using MAR.API.MortgageCalculator.Model.Dto;
using MAR.API.MortgageCalculator.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        private Guid _transactionId;
        /// <summary>
        /// Domain API transaction id
        /// </summary>
        protected Guid TransactionId => _transactionId;

        private const string TransactionIdKey = "TransactionId";

        /// <summary>
        /// Constructor
        /// </summary>
        public DomainBaseController()
        {
            SetTransactionId();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult GetResultForMortageCalculation(IMortgageCalculationResult result)
        {
            if (result.IsSuccessful)
            {
                return Ok(new ApiResponse<object>()
                {
                    ResponseDateTime = DateTime.UtcNow,
                    APIVersion = GetApiVersion(),
                    ApplicationName = GetApplicationName(),
                    TransactionId = TransactionId,
                    Data = result
                });
            }
            if (result.ValidationErrors?.Any() == true)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ApiResponse<object>()
                {
                    ResponseDateTime = DateTime.UtcNow,
                    APIVersion = GetApiVersion(),
                    ApplicationName = GetApplicationName(),
                    TransactionId = TransactionId,
                    Data = result
                });
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<object>()
            {
                ResponseDateTime = DateTime.UtcNow,
                APIVersion = GetApiVersion(),
                ApplicationName = GetApplicationName(),
                TransactionId = TransactionId,
                Data = result
            });
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
            return "1.0.1";
        }

        /// <summary>
        /// Gets API user friendly application name
        /// </summary>
        /// <returns></returns>
        protected string GetApplicationName()
        {
            //return _appSettings.ApplicationName;
            return "Mortgage Calculator API";
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
