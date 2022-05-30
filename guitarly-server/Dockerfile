FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY "guitarly.sln" .
COPY "Api/Api.csproj" "./Api/"
COPY "Models/Models.csproj" "./Models/"
COPY "ApplicationCore/ApplicationCore.csproj" "./ApplicationCore/"
# ↓ not sure it is ok
COPY "Data/Data.csproj" "./Data/"
COPY "ImageFactory/ImageFactory.csproj" "./ImageFactory/"
COPY "Parsers/Parsers.csproj" "./Parsers/"
RUN dotnet restore
COPY . .
WORKDIR /src/Api
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]