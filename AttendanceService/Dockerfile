FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AttendanceService/AttendanceService.csproj", "AttendanceService/"]
COPY ["Shered/Shered.csproj", "Shered/"]
RUN dotnet restore "AttendanceService/AttendanceService.csproj"

COPY . .
WORKDIR "/src/AttendanceService"
RUN dotnet build "AttendanceService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AttendanceService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AttendanceService.dll"]
