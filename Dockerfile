FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
FROM node:16-alpine as build-node
WORKDIR /client_app
COPY client_app/package.json .
COPY client_app/package-lock.json .
RUN npm install
COPY client_app/ .
RUN npm run build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV BuildingDocker true
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .
WORKDIR "/src/."
RUN dotnet build -c Release -o /app
FROM build AS publish
RUN dotnet publish -c Release -o /app
#FROM build AS publish
#RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build-node /client_app/dist ./client_app/
ENTRYPOINT ["dotnet", "CRAppWebApi.dll"]