FROM mcr.microsoft.com/dotnet/aspnet:6.0.24-alpine3.18 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0.416-alpine3.18 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/Web.Services
RUN dotnet build --self-contained --runtime linux-musl-x64 -c Release -o /app

FROM build AS publish
RUN dotnet publish --self-contained --runtime linux-musl-x64 -c Release -o /app

FROM base AS final
WORKDIR /app
RUN apk upgrade --available

COPY --from=publish /app .
RUN ls /app/App_Data

COPY ./GUI /app/GUI

ENV APP_USER hms_user

RUN adduser \
  --disabled-password \
  --home /app \
  --gecos '' $APP_USER \
  && chown -R $APP_USER:$APP_USER /app
USER $APP_USER:$APP_USER

ENTRYPOINT ["dotnet", "Web.Services.dll", "--urls", "http://0.0.0.0:8080"]
