using FluentValidation;
using MAR.API.MortgageCalculator.Model.Requests;

namespace MAR.API.MortgageCalculator.Logic.Validators
{
    public class MortgageCalculationRequestValidator : AbstractValidator<MortgageCalculationRequest>
    {
        public MortgageCalculationRequestValidator()
        {
            RuleFor(r => r.APR).GreaterThan(0.00M);
            RuleFor(r => r.DownPaymentPercent).GreaterThan(0.00M);
            RuleFor(r => r.HOAMonthly).GreaterThanOrEqualTo(0.00M);
            RuleFor(r => r.HomeownerInsuranceRate).GreaterThan(0.00M);
            RuleFor(r => r.LoanTermYears).GreaterThan(0);
            RuleFor(r => r.PropertyTaxRate).GreaterThan(0.00M);
            RuleFor(r => r.PurchasePrice).GreaterThan(0.00M);
        }
    }
}
