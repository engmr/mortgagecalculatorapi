using FluentValidation;
using MAR.API.MortgageCalculator.Localization;
using MAR.API.MortgageCalculator.Model.Requests;
using Microsoft.Extensions.Localization;

namespace MAR.API.MortgageCalculator.Logic.Validators
{
    public class MortgageCalculationRequestValidator : AbstractValidator<MortgageCalculationRequest>
    {
        public MortgageCalculationRequestValidator(IStringLocalizer<ValidationMessages> localizer)
        {
            RuleFor(r => r.APR).GreaterThanOrEqualTo(0.00M).WithMessage(l => localizer["APRIsInvalid"]);
            RuleFor(r => r.DownPaymentPercent).GreaterThanOrEqualTo(0.00M);
            RuleFor(r => r.HOAMonthly).GreaterThanOrEqualTo(0.00M);
            RuleFor(r => r.HomeownerInsuranceRate).GreaterThanOrEqualTo(0.00M);
            RuleFor(r => r.LoanTermYears).GreaterThanOrEqualTo(0);
            RuleFor(r => r.PropertyTaxRate).GreaterThanOrEqualTo(0.00M);
            RuleFor(r => r.PurchasePrice).GreaterThanOrEqualTo(0.00M);
        }
    }
}
