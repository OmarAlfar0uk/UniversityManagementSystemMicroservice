FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AcademicService/AcademicService.csproj", "AcademicService/"]
COPY ["Shered/Shered.csproj", "Shered/"]
RUN dotnet restore "AcademicService/AcademicService.csproj"

COPY . .
WORKDIR "/src/AcademicService"
RUN dotnet build "AcademicService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AcademicService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AcademicService.dll"]
