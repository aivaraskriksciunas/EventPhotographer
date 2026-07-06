# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EventPhotographer/EventPhotographer.csproj", "EventPhotographer/"]
COPY ["EventPhotographer.Core/EventPhotographer.Core.csproj", "EventPhotographer.Core/"]
COPY ["EventPhotographer.UseCases/EventPhotographer.UseCases.csproj", "EventPhotographer.UseCases/"]
RUN dotnet restore "./EventPhotographer/EventPhotographer.csproj"
COPY . .
WORKDIR "/src/EventPhotographer"
RUN dotnet build "./EventPhotographer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Builds a migration bundle
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS bundle
WORKDIR /src
COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet restore EventPhotographer/EventPhotographer.csproj

RUN dotnet ef migrations bundle \
	--project=EventPhotographer.Core/EventPhotographer.Core.csproj \
	--startup-project=EventPhotographer/EventPhotographer.csproj \
	--output=/bundle \
	--configuration=Release \
	--self-contained

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EventPhotographer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=bundle /bundle /app/efbundle
ENTRYPOINT ["/bin/sh", "-c", "/app/efbundle --connection=\"$ConnectionStrings__DefaultConnection\" && dotnet EventPhotographer.dll"]
