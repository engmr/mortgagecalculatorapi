using MAR.API.MortgageCalculator.Model.Interfaces;

namespace MAR.API.MortgageCalculator.Logic.Interfaces
{
    public interface ICalculateNetCalculatorProvider : IMortgageCalculatorProvider
    {
        IMortgageCalculationResult ParseHtmlIntoResult(string html);
    }
}
