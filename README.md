# mortgagecalculatorapi
A simple .NET Core calculator web API for a mortgage.

DISCLAIMER: This API is not meant to provide financial advice for those seeking a mortgage nor be for commercial use.

## Prerequisites
1. .NET Core 5.0 or later installed

After building at least once, to run the API locally:
```
cd <yourGitFolderWhereSlnIsAt>/MAR.API.MortgageCalculator
dotnet run MAR.API.MortgageCalculator.dll
```

## API Response
All calls (non-429 code) will return a response body with this format:
```
{
    "responseDateTime": "<UTC date time>",
    "apiVersion": "<current API Version>",
    "applicationName": "Mortgage Calculator API",
    "transactionId": "<a Guid>",
    "data": <null OR object>
}
```

### Sample API Request/Response
Version: 1.0.2  
Route: POST /calculate/free  
Content-Type: application/json

#### Request
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
#### Response
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

## Logging
This API has Serilog as its logging provider and will output to the console (limited) and more detailed to a rolling txt file (/log folder). Configuration is in the appsettings.json file.

## Rate Limiting
This API has rate limiting (AspNetRateLimit) implemented with configuration in the appsettings.json file.

## User Secrets
This API uses User Secrets. Right-click on the MAR.API.MortgageCalculator .csproj and select "Manage User Secrets".  
  
This will create (if first time ever) / open the secrets.json file for you to add overrides for the appsettings.json file.  
> e.g. AppSettings:PublicPaidAccessUserId

### Sample secrets.json
```
{
    "AppSettings:PublicPaidAccessUserId": "5db365cb-0bf2-4cad-8245-b56988078a3b",
    "AppSettings:PublicPaidAccessUserPassword": "yourCoolSecretPassword"
}
```

## Authorization
Some endpoints will require authorization. In the absence of a dedicated authorization service, the API has an AuthorizeController to handle issuing a token for use on its endpoints that require authorization.
  
### Issuing an AuthorizationToken
Route: POST /authorize/token/issue  
Headers:  
  - ClientId (Guid)
  - Password (string)
#### Token Issued Successfully Response
```
{
    "responseDateTime": "2021-03-20T16:41:54.829069Z",
    "apiVersion": "1.0.3",
    "applicationName": "Mortgage Calculator API",
    "transactionId": "58ad53d1-4414-4c3e-9cc4-77af9960d06f",
    "data": "1a1c2755-1090-4ac5-8c7a-8c1475abee1f"
}
```
The response.data is a string that is the authorization token to be used.  
  
NOTE: As of API version 1.0.3, only one user will exist in the authorization domain that can be issued a token. That user's credentials come from <AppSettings> PublicPaidAccessUserId / PublicPaidAccessUserPassword and get loaded at application startup only.

### Passing an AuthorizationToken to an Endpoint
Required Headers:
  - ClientId (guid)
  - AuthorizationToken (string)

## Mortgage Calculation Flow Diagram
![Mortgage Calculation Flow Diagram](https://github.com/engmr/mortgagecalculatorapi/blob/master/MortgageCalcAPI_Calculation_Flow_Diagram.png?raw=true)

