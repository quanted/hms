FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY HMS.sln ./
COPY Web.Services/Web.Services.csproj Web.Services/
COPY Web.Services.Tests/Web.Services.Tests.csproj Web.Services.Tests/
COPY Precipitation/Precipitation.csproj Precipitation/
COPY Precipitation.Tests/Precipitation.Tests.csproj Precipitation.Tests/
COPY Data.Simulate/Data.Simulate.csproj Data.Simulate/
COPY Data/Data.csproj Data/
COPY Data.Tests/Data.Tests.csproj Data.Tests/
COPY Utilities/Utilities.csproj Utilities/
COPY Utilities.Tests/Utilities.Tests.csproj Utilities.Tests/
COPY Data.Source/Data.Source.csproj Data.Source/
COPY Data.Source.Tests/Data.Source.Tests.csproj Data.Source.Tests/
COPY SoilMoisture/SoilMoisture.csproj SoilMoisture/
COPY Temperature/Temperature.csproj Temperature/
COPY SubSurfaceFlow/SubSurfaceFlow.csproj SubSurfaceFlow/
COPY Solar/Solar.csproj Solar/
COPY Pressure/Pressure.csproj Pressure/
COPY Radiation/Radiation.csproj Radiation/
COPY Wind/Wind.csproj Wind/
COPY DewPoint/DewPoint.csproj DewPoint/
COPY Humidity/Humidity.csproj Humidity/
COPY Evapotranspiration/Evapotranspiration.csproj Evapotranspiration/
COPY SurfaceRunoff/SurfaceRunoff.csproj SurfaceRunoff/
COPY GIS.Operations/GIS.Operations.csproj GIS.Operations/
COPY Solar.Tests/Solar.Tests.csproj Solar.Tests/
COPY Stream.Hydrology/AQUATOX/Stream.Hydrology.AQUATOX.csproj Stream.Hydrology/AQUATOX/
COPY Stream.Hydrology/AQUATOX.UnitTests/Stream.Hydrology.AQUATOX.UnitTests.csproj Stream.Hydrology/AQUATOX.UnitTests/
COPY Data.Simulate.AQUATOX/Data.Simulate.AQUATOX.csproj Data.Simulate.AQUATOX/
COPY Nutrients/Nutrients.AQUATOX/Nutrients.AQUATOX.csproj Nutrients/Nutrients.AQUATOX/
COPY Nutrients/Nutrients.AQUATOX.UnitTests/Nutrients.AQUATOX.UnitTests.csproj Nutrients/Nutrients.AQUATOX.UnitTests/
COPY OrganicMatter/OrganicMatter.AQUATOX/OrganicMatter.AQUATOX.csproj OrganicMatter/OrganicMatter.AQUATOX/
COPY OrganicMatter/OrganicMatter.AQUATOX.UnitTests/OrganicMatter.AQUATOX.UnitTests.csproj OrganicMatter/OrganicMatter.AQUATOX.UnitTests/
COPY Chemicals/Chemicals.AQUATOX/Chemicals.AQUATOX.csproj Chemicals/Chemicals.AQUATOX/
COPY Chemicals/Chemicals.AQUATOX.UnitTests/Chemicals.AQUATOX.UnitTests.csproj Chemicals/Chemicals.AQUATOX.UnitTests/
COPY Diagenesis/Diagenesis.AQUATOX/Diagenesis.AQUATOX.csproj Diagenesis/Diagenesis.AQUATOX/
COPY Diagenesis/Diagenesis.AQUATOX.UnitTests/Diagenesis.AQUATOX.UnitTests.csproj Diagenesis/Diagenesis.AQUATOX.UnitTests/
COPY WatershedDelineation/WatershedDelineation.csproj WatershedDelineation/
COPY ContaminantLoader/ContaminantLoader.csproj ContaminantLoader/

RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Web.Services
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY . /app
COPY --from=publish /app .
# COPY test_reports/ /src/
# COPY . /src
RUN ls /app/App_Data
RUN mkdir -p /src/Web.Services.Tests/bin/Release/netcoreapp3.0
# ENTRYPOINT ["/bin/bash", "/src/runtests_start.sh"]
ENTRYPOINT ["dotnet", "Web.Services.dll", "--urls", "http://0.0.0.0:80"]
