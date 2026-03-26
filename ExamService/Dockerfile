FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ExamService/ExamService.csproj", "ExamService/"]
COPY ["Shered/Shered.csproj", "Shered/"]
RUN dotnet restore "ExamService/ExamService.csproj"

COPY . .
WORKDIR "/src/ExamService"
RUN dotnet build "ExamService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExamService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExamService.dll"]
