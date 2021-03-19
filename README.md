# mortgagecalculatorapi
A simple .NET Core calculator web API for a mortgage.

## Prerequisites
1. .NET Core 5.0 or later installed

After building at once, to run the API locally:
```
cd <yourGitFolder>/mortgagecalculatorapi/MAR.API.MortgageCalculator
dotnet run MAR.API.MortgageCalculator.dll
```

## Logging
This API has Serilog as its logging provider and will output to the console (limited) and more detailed to a rolling txt file (/log folder). Configuration is in the appsettings.json file.

## Rate Limiting
This API has rate limiting (AspNetRateLimit) implemented with configuration in the appsettings.json file.

## Mortgage Calculation Flow Diagram
![Mortgage Calculation Flow Diagram](https://github.com/engmr/mortgagecalculatorapi/blob/master/MortgageCalcAPI_Calculation_Flow_Diagram.png?raw=true)

## Sample Request/Response
Version 1.0.2

### Request
```
{
  "purchasePrice": 100000,
  "apr": 2.75,
  "loanTermYears": 30,
  "downPaymentPercent": 20,
  "propertyTaxRate": 1.15,
  "homeownerInsuranceRate": 0.22,
  "hoaMonthly": 10
}
```
### Response
```
{
    "responseDateTime": "2021-03-19T23:38:03.8564816Z",
    "apiVersion": "1.0.2",
    "applicationName": "Mortgage Calculator API",
    "transactionId": "a1d5a498-9c7a-4413-ad7c-6134aeaa9c93",
    "data": {
        "request": {
            "purchasePrice": 100000,
            "apr": 2.75,
            "loanTermYears": 30,
            "downPaymentPercent": 20,
            "propertyTaxRate": 1.15,
            "homeownerInsuranceRate": 0.22,
            "hoaMonthly": 10
        },
        "errors": [],
        "validationErrors": [],
        "isSuccessful": true,
        "mortgagePaymentyMonthly": 326.59,
        "propertyTaxPaymentMonthly": 95.83,
        "homeownersInsurancePaymentMonthly": 18.33,
        "totalMonthlyPayment": 450.75,
        "downPayment": 20000,
        "termInterestPaid": 37573.46,
        "totalCostOfLoan": 117573.46,
        "hoaAnnual": 120
    }
}
```
