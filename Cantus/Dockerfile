FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cantus/Cantus.csproj", "."]
RUN dotnet restore "./Cantus.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Cantus/Cantus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cantus/Cantus.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cantus.dll"]