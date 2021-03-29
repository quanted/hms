FROM mcr.microsoft.com/dotnet/sdk:3.1 AS base
WORKDIR /app
EXPOSE 80

#FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/Web.Services
RUN dotnet build -c Release -o /app

#FROM build AS publish
RUN dotnet publish -c Release -o /app

#FROM base AS final
WORKDIR /app
COPY . /app
#COPY --from=publish /app .
RUN ls /app/App_Data
RUN mkdir -p /src/Web.Services.Tests/bin/Release/netcoreapp3.1
ENTRYPOINT ["dotnet", "Web.Services.dll", "--urls", "http://0.0.0.0:80"]