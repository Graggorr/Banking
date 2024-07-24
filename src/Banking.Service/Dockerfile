FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Banking.Service/Banking.Service.csproj", "src/Banking.Service/"]
COPY ["src/Banking.Application/Banking.Application.csproj", "src/Banking.Application/"]
COPY ["src/Banking.Domain/Banking.Domain.csproj", "src/Banking.Domain/"]
COPY ["src/Banking.Infrastructure/Banking.Infrastructure.csproj", "src/Banking.Infrastructure/"]
RUN dotnet restore "./src/Banking.Service/Banking.Service.csproj"
COPY . .
WORKDIR "/src/src/Banking.Service"
RUN dotnet build "./Banking.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Banking.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Banking.Service.dll"]