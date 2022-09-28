FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine3.15 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.15 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/Web.Services
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app

COPY --from=publish /app .
RUN ls /app/App_Data
#RUN mkdir -p /src/Web.Services.Tests/bin/Release/net5.0

COPY ./GUI /app/GUI

ENV APP_USER hms_user

RUN adduser \
  --disabled-password \
  --home /app \
  --gecos '' $APP_USER \
  && chown -R $APP_USER:$APP_USER /app
USER $APP_USER:$APP_USER

ENTRYPOINT ["dotnet", "Web.Services.dll", "--urls", "http://0.0.0.0:8080"]