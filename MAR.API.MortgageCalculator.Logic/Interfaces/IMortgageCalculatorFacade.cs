using MAR.API.MortgageCalculator.Model.Interfaces;

namespace MAR.API.MortgageCalculator.Logic.Interfaces
{
    public interface IMortgageCalculatorFacade
    {
        IMortgageCalculationResult GetMortgageCalculation(IMortgageCalculationRequest request);
    }
}
