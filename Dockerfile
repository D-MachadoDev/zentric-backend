# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["Zentric.Domain/Zentric.Domain.csproj", "Zentric.Domain/"]
COPY ["Zentric.Application/Zentric.Application.csproj", "Zentric.Application/"]
COPY ["Zentric.Infrastructure/Zentric.Infrastructure.csproj", "Zentric.Infrastructure/"]
COPY ["Zentric.Api/Zentric.Api.csproj", "Zentric.Api/"]
COPY ["Zentric.Tests/Zentric.Tests.csproj", "Zentric.Tests/"]
COPY ["Zentric.slnx", "./"]

RUN dotnet restore "Zentric.slnx"

# Copy full source and build
COPY . .
WORKDIR "/src/Zentric.Api"
RUN dotnet publish "Zentric.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Zentric.Api.dll"]
