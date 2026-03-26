FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProgressService/ProgressService.csproj", "ProgressService/"]
COPY ["Shered/Shered.csproj", "Shered/"]
RUN dotnet restore "ProgressService/ProgressService.csproj"

COPY . .
WORKDIR "/src/ProgressService"
RUN dotnet build "ProgressService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProgressService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProgressService.dll"]
