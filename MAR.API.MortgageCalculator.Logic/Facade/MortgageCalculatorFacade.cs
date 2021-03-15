using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Model.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace MAR.API.MortgageCalculator.Logic.Facade
{
    public class MortgageCalculatorFacade : IMortgageCalculatorFacade
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<MortgageCalculatorFacade> _logger;
        private IMortgageCalculatorProviderFactory _mortgageCalculatorProviderFactory;

        public MortgageCalculatorFacade(ILoggerFactory loggerFactory, IMortgageCalculatorProviderFactory mortgageCalculatorProviderFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<MortgageCalculatorFacade>();
            _mortgageCalculatorProviderFactory = mortgageCalculatorProviderFactory ?? throw new ArgumentNullException(nameof(mortgageCalculatorProviderFactory));
        }

        public IMortgageCalculationResult GetMortgageCalculation(IMortgageCalculationRequest request)
        {
            _logger.LogDebug($"{nameof(GetMortgageCalculation)} Request received");
            var provider = _mortgageCalculatorProviderFactory.GetProvider(request);
            return provider.PerformCalculation();
        }
    }
}
