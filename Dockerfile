# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . . 
RUN dotnet restore "Rfm.Api/Rfm.Api.csproj"

# Build and publish
RUN dotnet publish "Rfm.Api/Rfm.Api.csproj" -c Release -o /out

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy from build stage
COPY --from=build /out ./

# Set entrypoint
ENTRYPOINT ["dotnet", "Rfm.Api.dll"]
