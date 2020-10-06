FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS prep-env
COPY . ./
RUN mkdir /proj && cp --parents */*.csproj /proj

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY --from=prep-env ./proj .
RUN dotnet restore ./WarehouseSystem.Api

# Copy everything else and build
COPY . ./
RUN dotnet publish ./WarehouseSystem.Api -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "./WarehouseSystem.Api.dll"]
