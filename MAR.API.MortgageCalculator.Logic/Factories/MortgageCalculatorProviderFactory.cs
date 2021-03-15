using MAR.API.MortgageCalculator.Localization;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Logic.Providers;
using MAR.API.MortgageCalculator.Model.Interfaces;
using MAR.API.MortgageCalculator.Model.Requests;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;

namespace MAR.API.MortgageCalculator.Logic.Factories
{
    public class MortgageCalculatorProviderFactory : IMortgageCalculatorProviderFactory
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<MortgageCalculatorProviderFactory> _logger;
        private IStringLocalizer<ErrorMessages> _errorMessageLocalizer;
        private IStringLocalizer<ValidationMessages> _validationMessageLocalizer;
        private IHttpClientProvider _httpClientProvider;

        public MortgageCalculatorProviderFactory(ILoggerFactory loggerFactory, IStringLocalizer<ErrorMessages> errorMessageLocalizer, IStringLocalizer<ValidationMessages> validationMessageLocalizer,
            IHttpClientProvider httpClientProvider)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<MortgageCalculatorProviderFactory>();
            _errorMessageLocalizer = errorMessageLocalizer ?? throw new ArgumentNullException(nameof(errorMessageLocalizer));
            _validationMessageLocalizer = validationMessageLocalizer ?? throw new ArgumentNullException(nameof(validationMessageLocalizer));
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
        }

        public IMortgageCalculatorProvider GetProvider(IMortgageCalculationRequest request)
        {
            switch (request)
            {
                case MortgageCalculationRequest mcRequest:
                    _logger.LogDebug($"{nameof(GetProvider)} is producing a {nameof(CalculateNetCalculatorProvider)} provider");
                    return new CalculateNetCalculatorProvider(_loggerFactory, _validationMessageLocalizer, _errorMessageLocalizer, _httpClientProvider, mcRequest);
                default:
                    throw new NotImplementedException(_errorMessageLocalizer["MortgageCalculationRequestTypeNotSupported", request.GetType().Name]);
            }
        }
    }
}
