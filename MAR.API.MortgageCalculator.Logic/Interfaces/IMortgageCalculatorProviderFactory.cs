using MAR.API.MortgageCalculator.Model.Interfaces;

namespace MAR.API.MortgageCalculator.Logic.Interfaces
{
    public interface IMortgageCalculatorProviderFactory
    {
        IMortgageCalculatorProvider GetProvider(IMortgageCalculationRequest request);
    }
}
