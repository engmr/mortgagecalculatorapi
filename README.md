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

## UML Diagram
Coming soon...
