#Build API from code
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
WORKDIR /src
COPY . .
RUN dotnet restore MAR.API.MortgageCalculator/MAR.API.MortgageCalculator.csproj
RUN dotnet build MAR.API.MortgageCalculator/MAR.API.MortgageCalculator.csproj
#RUN dotnet test
RUN dotnet publish MAR.API.MortgageCalculator/MAR.API.MortgageCalculator.csproj -c Release -o /app

# Create API runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS api
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "MAR.API.MortgageCalculator.dll"]