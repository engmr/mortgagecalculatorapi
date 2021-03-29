using MAR.API.MortgageCalculator.Model.Requests;
using System.Collections.Generic;

namespace MAR.API.MortgageCalculator.Model.Interfaces
{
    public interface IBulkMortgageCalculationRequest
    {
        List<MortgageCalculationRequest> Requests { get; set; }
    }
}
