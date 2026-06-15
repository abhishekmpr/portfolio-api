FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Install Kerberos library required by Npgsql
RUN apt-get update && \
    apt-get install -y libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PortfolioApi.dll"]