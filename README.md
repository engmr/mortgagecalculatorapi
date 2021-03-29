# mortgagecalculatorapi
A simple .NET Core calculator web API for a mortgage.

DISCLAIMER: This API is not meant to provide financial advice for those seeking a mortgage nor be for commercial use.

## Prerequisites
1. .NET 5.0 or later installed
2. HTTPS self-signed certficate (localhost)
```
# If you already have a self-signed cert on your machine and want to nuke it...
dotnet dev-certs https --clean

# enable self-signed Https cert on machine (replace %userprofile% with $env:USERPROFILE in PowerShell)
dotnet dev-certs https -ep %userprofile%\.aspnet\https\aspnetapp.pfx -p yourPasswordHere
dotnet dev-certs https --trust
```

## Running the API locally

### Option 1 - Publish into Docker
See Docker section

### Option 2 - IIS Express
Debug using Visual Studio

### Option 3 - Local Windows Process
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
    "AppSettings:PublicPaidAccessUserPassword": "yourCoolSecretPassword",
    "IpRateLimiting:ClientWhiteList:0": "MyIdHere"
}
```

## Docker
This API has been setup to use Docker for creating an image for use in a container.

### PowerShell ScriptBlocks

#### Build Image
```
$BuildMortgageCalculatorAPIDockerImage =
{
	cd $MortgageCalculatorAPISourceDir
	docker build -t $MortgageCalculatorAPIImageName -f Dockerfile .
	docker images
}
# where $MortgageCalculatorAPIImageName is the image name (e.g. mycoolapi)
```

#### Run Image
```
$RunMortgageCalculatorAPIDockerImage =
{
	docker run -d -p 8080:5000 -p 8081:5001 --env-file=$MortgageCalculatorAPIDockerEnvFile -v $env:USERPROFILE\.aspnet\https:/https/ --name $MortgageCalculatorAPIContainerName $MortgageCalculatorAPIImageName
	docker ps
}
<# where 
    $MortgageCalculatorAPIDockerEnvFile is docker environment file (usually the DockerEnv.txt file in the API's solution directory)
    $MortgageCalculatorAPIContainerName is the container name you want (e.g. mycoolapi-container)
#>
```

### Sample DockerEnv.txt
The `--env-file` text file will contain the environment variables the application needs to run (e.g. AppSettings overrides). Placing a file of exact name 'DockerEnv.txt' in the sln directory can be done since it is not tracked in source control. This file can then be passed into the `docker run` command for use by the API.
  
```
#Environment AppSettings Variables
AppSettings__PublicPaidAccessUserId=00000000-0000-0000-0000-000000000000
AppSettings__PublicPaidAccessUserPassword=yourPasswordHere
#IPRateLimiting Variables
IpRateLimiting__ClientWhiteList__0=SomeIdHere
#ASP.NET CORE Variables
ASPNETCORE_Kestrel__Certificates__Default__Password=yourDevCertsHttpsPassword
```
  
After the container is running, navigate to https://localhost:8081/health/check to verify it is running properly. Redirection to https:// is active.

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

