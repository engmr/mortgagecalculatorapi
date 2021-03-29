using MAR.API.MortgageCalculator.Model.Interfaces;
using System.Collections.Generic;

namespace MAR.API.MortgageCalculator.Model.Requests
{
    public class BulkMortgageCalculationRequest : IBulkMortgageCalculationRequest
    {
        public List<MortgageCalculationRequest> Requests { get; set; }
    }
}
