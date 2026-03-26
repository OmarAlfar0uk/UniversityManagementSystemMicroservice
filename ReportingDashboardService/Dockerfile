FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ReportingDashboardService/ReportingDashboardService.csproj", "ReportingDashboardService/"]
COPY ["Shered/Shered.csproj", "Shered/"]
RUN dotnet restore "ReportingDashboardService/ReportingDashboardService.csproj"

COPY . .
WORKDIR "/src/ReportingDashboardService"
RUN dotnet build "ReportingDashboardService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportingDashboardService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportingDashboardService.dll"]
