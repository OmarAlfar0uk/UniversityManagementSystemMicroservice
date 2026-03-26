FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GradeService/GradeService.csproj", "GradeService/"]
COPY ["Shered/Shered.csproj", "Shered/"]
RUN dotnet restore "GradeService/GradeService.csproj"

COPY . .
WORKDIR "/src/GradeService"
RUN dotnet build "GradeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GradeService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GradeService.dll"]
