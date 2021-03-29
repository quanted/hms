FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/Web.Services
RUN dotnet build -c Release -o /src/build
WORKDIR /src/build
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/sdk:3.1
EXPOSE 80
WORKDIR /app
COPY . /app
COPY --from=build /app .
RUN ls /app/App_Data
RUN mkdir -p /src/Web.Services.Tests/bin/Release/netcoreapp3.1
ENTRYPOINT ["dotnet", "Web.Services.dll", "--urls", "http://0.0.0.0:80"]
