FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/sdk:3.1
EXPOSE 80
WORKDIR /app
COPY --from=build-env /app/out .
RUN mkdir -p /app/Web.Services.Tests/bin/Release/netcoreapp3.1
ENTRYPOINT ["dotnet", "Web.Services.dll", "--urls", "http://0.0.0.0:80"]