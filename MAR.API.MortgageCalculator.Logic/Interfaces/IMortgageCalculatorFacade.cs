using MAR.API.MortgageCalculator.Model.Interfaces;
using System.Collections.Generic;

namespace MAR.API.MortgageCalculator.Logic.Interfaces
{
    public interface IMortgageCalculatorFacade
    {
        IMortgageCalculationResult GetMortgageCalculation(IMortgageCalculationRequest request);
        IEnumerable<IMortgageCalculationResult> GetMortgageCalculations(IBulkMortgageCalculationRequest request);
    }
}
