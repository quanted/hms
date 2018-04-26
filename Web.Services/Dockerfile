FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY HMS.sln ./
COPY Web.Services/Web.Services.csproj Web.Services/
COPY Precipitation/Precipitation.csproj Precipitation/
COPY Data.Simulate/Data.Simulate.csproj Data.Simulate/
COPY Data/Data.csproj Data/
COPY Utilities/Utilities.csproj Utilities/
COPY Data.Source/Data.Source.csproj Data.Source/
COPY SoilMoisture/SoilMoisture.csproj SoilMoisture/
COPY Temperature/Temperature.csproj Temperature/
COPY SubSurfaceFlow/SubSurfaceFlow.csproj SubSurfaceFlow/
COPY Solar/Solar.csproj Solar/
COPY Evapotranspiration/Evapotranspiration.csproj Evapotranspiration/
COPY SurfaceRunoff/SurfaceRunoff.csproj SurfaceRunoff/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Web.Services
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.Services.dll"]
