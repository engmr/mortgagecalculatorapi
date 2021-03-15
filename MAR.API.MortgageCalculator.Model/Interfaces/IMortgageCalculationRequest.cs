namespace MAR.API.MortgageCalculator.Model.Interfaces
{
    public interface IMortgageCalculationRequest
    {
        decimal PurchasePrice { get; set; }
        decimal APR { get; set; }
        int LoanTermYears { get; set; }
        decimal DownPaymentPercent { get; set; }
        decimal PropertyTaxRate { get; set; }
        decimal HomeownerInsuranceRate { get; set; }
        decimal HOAMonthly { get; set; }
    }
}
