#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 6060
ENV ASPNETCORE_URLS=http://*:6060
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ContactsUpdateProducer.Api/FIAP.TechChallenge.ContactsUpdateProducer.Api.csproj", "ContactsUpdateProducer.Api/"]
COPY ["ContactsUpdateProducer.Application/FIAP.TechChallenge.ContactsUpdateProducer.Application.csproj", "ContactsUpdateProducer.Application/"]
COPY ["ContactsUpdateProducer.Domain/FIAP.TechChallenge.ContactsUpdateProducer.Domain.csproj", "ContactsUpdateProducer.Domain/"]
COPY ["ContactsUpdateProducer.Infrastructure/FIAP.TechChallenge.ContactsUpdateProducer.Infrastructure.csproj", "ContactsUpdateProducer.Infrastructure/"]
COPY ["ContactsUpdateProducer.Integrations/FIAP.TechChallenge.ContactsUpdateProducer.Integrations.csproj", "ContactsUpdateProducer.Integrations/"]
RUN dotnet restore "./ContactsUpdateProducer.Api/FIAP.TechChallenge.ContactsUpdateProducer.Api.csproj"
COPY . .
WORKDIR "/src/ContactsUpdateProducer.Api"
RUN dotnet build "./FIAP.TechChallenge.ContactsUpdateProducer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FIAP.TechChallenge.ContactsUpdateProducer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FIAP.TechChallenge.ContactsUpdateProducer.Api.dll"]