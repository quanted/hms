FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.15 AS base
WORKDIR /app
EXPOSE 80

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
RUN mkdir -p /src/Web.Services.Tests/bin/Release/net5.0

ENV APP_USER hms_user
#ENV APP_GROUP hms_app
#RUN addgroup -S $APP_GROUP && adduser -S $APP_USER -G $APP_GROUP

RUN adduser \
  --disabled-password \
  --home /app \
  --gecos '' $APP_USER \
  && chown -R $APP_USER /app
USER $APP_USER

ENTRYPOINT ["dotnet", "Web.Services.dll", "--urls", "http://0.0.0.0:80"]