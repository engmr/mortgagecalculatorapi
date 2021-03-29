#Build API from code
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
WORKDIR /src
COPY . .
RUN dotnet restore MAR.API.MortgageCalculator/MAR.API.MortgageCalculator.csproj
#RUN dotnet test
RUN dotnet publish MAR.API.MortgageCalculator/MAR.API.MortgageCalculator.csproj -c Release -o /app

# Create API runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS api
WORKDIR /app
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS="https://*:5001;http://*:5000"
ENV ASPNETCORE_HTTPS_PORT=8081
#ENV ASPNETCORE_Kestrel__Certificates__Default__Password=yourPassword
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx

COPY --from=builder /app .
ENTRYPOINT ["dotnet", "MAR.API.MortgageCalculator.dll"]