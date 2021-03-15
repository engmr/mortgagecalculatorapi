using FluentValidation.Results;
using MAR.API.MortgageCalculator.Model.Interfaces;

namespace MAR.API.MortgageCalculator.Logic.Interfaces
{
    public interface IMortgageCalculatorProvider
    {
        ValidationResult ValidateRequest();
        IMortgageCalculationResult PerformCalculation();
    }
}
