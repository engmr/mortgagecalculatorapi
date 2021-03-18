using FluentValidation.Results;
using HtmlAgilityPack;
using MAR.API.MortgageCalculator.Localization;
using MAR.API.MortgageCalculator.Logic.Interfaces;
using MAR.API.MortgageCalculator.Logic.Validators;
using MAR.API.MortgageCalculator.Model.Interfaces;
using MAR.API.MortgageCalculator.Model.Requests;
using MAR.API.MortgageCalculator.Model.Results;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MAR.API.MortgageCalculator.Logic.Providers
{
    public class CalculateNetCalculatorProvider : ICalculateNetCalculatorProvider
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<CalculateNetCalculatorProvider> _logger;
        private IStringLocalizer<ValidationMessages> _validationMessageLocalizer;
        private IStringLocalizer<ErrorMessages> _errorMessageLocalizer;
        private IHttpClientProvider _httpClientProvider;
        private MortgageCalculationRequest _request;

        public CalculateNetCalculatorProvider(
            ILoggerFactory loggerFactory,
            IStringLocalizer<ValidationMessages> validationMessageLocalizer, 
            IStringLocalizer<ErrorMessages> errorMessageLocalizer,
            IHttpClientProvider httpClientProvider, 
            MortgageCalculationRequest request)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<CalculateNetCalculatorProvider>();
            _validationMessageLocalizer = validationMessageLocalizer ?? throw new ArgumentNullException(nameof(validationMessageLocalizer));
            _errorMessageLocalizer = errorMessageLocalizer ?? throw new ArgumentNullException(nameof(validationMessageLocalizer));
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public IMortgageCalculationResult ParseHtmlIntoResult(string html)
        {
            _logger.LogDebug($"{nameof(ParseHtmlIntoResult)} is being executed");
            var htmlPage = new HtmlDocument();
            htmlPage.LoadHtml(html);
            var mortgageInfoTable = htmlPage.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[3]/div[1]/table[1]/tr[1]/td[2]/table[1]/tr[1]/td[1]/table[1]");
            if (mortgageInfoTable == null)
            {
                return new MortgageCalculationResult(_request, null, new List<string>() { _errorMessageLocalizer["MortgageInfoTableMissing"] });
            }

            var mortgageInfoRows = mortgageInfoTable.SelectNodes("./tr");
            if (mortgageInfoRows.Any())
            {
                var mortgageCalculationResult = new MortgageCalculationResult(_request, null, null);
                mortgageCalculationResult.MortgagePaymentyMonthly = decimal.Parse(mortgageInfoRows[1].SelectSingleNode("./td[2]").InnerText, NumberStyles.Currency);
                mortgageCalculationResult.PropertyTaxPaymentMonthly = decimal.Parse(mortgageInfoRows[2].SelectSingleNode("./td[2]").InnerText, NumberStyles.Currency);
                mortgageCalculationResult.HomeownersInsurancePaymentMonthly = decimal.Parse(mortgageInfoRows[3].SelectSingleNode("./td[2]").InnerText, NumberStyles.Currency);
                if (mortgageCalculationResult.Request.HOAMonthly > 0)
                {
                    mortgageCalculationResult.DownPayment = decimal.Parse(mortgageInfoRows[9].SelectSingleNode("./td[2]").InnerText, NumberStyles.Currency);
                    mortgageCalculationResult.TermInterestPaid = decimal.Parse(mortgageInfoRows[11].SelectSingleNode("./td[2]").InnerText, NumberStyles.Currency);
                }
                else
                {
                    mortgageCalculationResult.DownPayment = decimal.Parse(mortgageInfoRows[8].SelectSingleNode("./td[2]").InnerText, NumberStyles.Currency);
                    mortgageCalculationResult.TermInterestPaid = decimal.Parse(mortgageInfoRows[10].SelectSingleNode("./td[2]").InnerText, NumberStyles.Currency);
                }
                return mortgageCalculationResult;
            }
            return new MortgageCalculationResult(_request, null, new List<string>() { _errorMessageLocalizer["MortgageInfoTableHadNoData"] });
        }

        public IMortgageCalculationResult PerformCalculation()
        {
            _logger.LogDebug($"{nameof(PerformCalculation)} is being executed");
            var validationResult = ValidateRequest();
            if (!validationResult.IsValid)
            {
                return new MortgageCalculationResult(_request, validationResult.Errors.Select(e => e.ErrorMessage).ToList(), null);
            }

            try
            {
                var responseHtml = GetRawHtml();
                return ParseHtmlIntoResult(responseHtml);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PerformCalculation)}");
                return new MortgageCalculationResult(_request, null, new List<string>() { ex.Message });
            }

        }

        public ValidationResult ValidateRequest()
        {
            _logger.LogDebug($"{nameof(ValidateRequest)} is being executed");
            var validator = new MortgageCalculationRequestValidator();
            return validator.Validate(_request);
        }

        private string FormatUrl(MortgageCalculationRequest request)
        {
            return FormatUrl(
                request.PurchasePrice.ToString(), request.LoanTermYears.ToString(), request.DownPaymentPercent.ToString(),
                request.APR.ToString(), request.PropertyTaxRate.ToString(), request.HomeownerInsuranceRate.ToString(),
                (request.HOAMonthly * 12).ToString()
                );
        }

        /// <summary>
        /// Get the calculator HTTP url based in inputs
        /// </summary>
        /// <param name="housePrice"></param>
        /// <param name="loanTermYears"></param>
        /// <param name="downPaymentPercent"></param>
        /// <param name="interestRate"></param>
        /// <param name="propertyTaxRate"></param>
        /// <param name="homeownerInsuranceRate"></param>
        /// <param name="hoaAnnual"></param>
        /// <returns>url</returns>
        private string FormatUrl(string housePrice, string loanTermYears, string downPaymentPercent, string interestRate, string propertyTaxRate, string homeownerInsuranceRate, string hoaAnnual)
        {
            return $"https://www.calculator.net/mortgage-calculator.html?chouseprice={housePrice}&cdownpayment={downPaymentPercent}&cdownpaymentunit=p&cloanterm={loanTermYears}&cinterestrate={interestRate}&cstartmonth=11&cstartyear=2020&caddoptional=1&cpropertytaxes={propertyTaxRate}&cpropertytaxesunit=p&chomeins={homeownerInsuranceRate}&chomeinsunit=p&cpmi=0&cpmiunit=d&choa={hoaAnnual}&choaunit=d&cothercost=0&cothercostunit=d&cmop=0&cptinc=0&chiinc=0&choainc=0&cocinc=0&cexma=0&cexmsm=11&cexmsy=2020&cexya=0&cexysm=11&cexysy=2020&cexoa=0&cexosm=11&cexosy=2020&caot=0&xa1=0&xm1=11&xy1=2020&xa2=0&xm2=11&xy2=2020&xa3=0&xm3=11&xy3=2020&xa4=0&xm4=11&xy4=2020&xa5=0&xm5=11&xy5=2020&xa6=0&xm6=11&xy6=2020&xa7=0&xm7=11&xy7=2020&xa8=0&xm8=11&xy8=2020&xa9=0&xm9=11&xy9=2020&xa10=0&xm10=11&xy10=2020&csbw=1&printit=0&x=59&y=18";
        }

        private string GetRawHtml()
        {
            _logger.LogDebug($"{nameof(GetRawHtml)} is being executed");
            var requestUrl = FormatUrl(_request);
            return _httpClientProvider.GetAsync(requestUrl).GetAwaiter().GetResult();
        }
    }
}
