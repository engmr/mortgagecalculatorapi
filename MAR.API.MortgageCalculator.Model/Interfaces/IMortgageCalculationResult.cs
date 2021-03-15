using MAR.API.MortgageCalculator.Model.Requests;
using System.Collections.Generic;

namespace MAR.API.MortgageCalculator.Model.Interfaces
{
    public interface IMortgageCalculationResult
    {
        MortgageCalculationRequest Request { get; }
        List<string> Errors { get; }
        List<string> ValidationErrors { get; }
        bool IsSuccessful { get; }
        decimal MortgagePaymentyMonthly { get; set; }
        decimal PropertyTaxPaymentMonthly { get; set; }
        decimal HomeownersInsurancePaymentMonthly { get; set; }
        decimal TotalMonthlyPayment { get; }

        decimal DownPayment { get; set; }
        decimal TermInterestPaid { get; set; }
        decimal TotalCostOfLoan { get; }
        decimal HOAAnnual { get; }
    }
}
