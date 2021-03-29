using MAR.API.MortgageCalculator.Localization;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Model.Interfaces;
using MAR.API.MortgageCalculator.Model.Requests;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MAR.API.MortgageCalculator.Logic.Facade
{
    public class MortgageCalculatorFacade : IMortgageCalculatorFacade
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<MortgageCalculatorFacade> _logger;
        private IMortgageCalculatorProviderFactory _mortgageCalculatorProviderFactory;
        private IStringLocalizer<ErrorMessages> _errorMessageLocalizer;

        public MortgageCalculatorFacade(ILoggerFactory loggerFactory, IMortgageCalculatorProviderFactory mortgageCalculatorProviderFactory, IStringLocalizer<ErrorMessages> errorMessageLocalizer)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<MortgageCalculatorFacade>();
            _mortgageCalculatorProviderFactory = mortgageCalculatorProviderFactory ?? throw new ArgumentNullException(nameof(mortgageCalculatorProviderFactory));
            _errorMessageLocalizer = errorMessageLocalizer ?? throw new ArgumentNullException(nameof(errorMessageLocalizer));
        }

        public IMortgageCalculationResult GetMortgageCalculation(IMortgageCalculationRequest request)
        {
            _logger.LogDebug($"{nameof(GetMortgageCalculation)} Request received");
            var provider = _mortgageCalculatorProviderFactory.GetProvider(request);
            return provider.PerformCalculation();
        }

        public IEnumerable<IMortgageCalculationResult> GetMortgageCalculations(IBulkMortgageCalculationRequest request)
        {
            _logger.LogDebug($"{nameof(GetMortgageCalculations)} Request received");
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.Requests == null)
            {
                throw new ArgumentNullException(nameof(request.Requests));
            }

            var response = new List<IMortgageCalculationResult>();
            switch (request)
            {
                case BulkMortgageCalculationRequest bmcRequest:
                    {
                        foreach (var singleRequest in request.Requests)
                        {
                            var provider = _mortgageCalculatorProviderFactory.GetProvider(singleRequest);
                            response.Add(provider.PerformCalculation());
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException(_errorMessageLocalizer["MortgageCalculationRequestTypeNotSupported", request.GetType().Name]);
            }

            return response;
        }
    }
}
