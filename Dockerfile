FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build


ARG BUILD_CONFIGURATION=Release

# COPY SRC
WORKDIR /src/KafkaConsumer
COPY ["./KafkaConsumer.sln", "./"]
COPY ["./Directory.Build.props", "./"]

RUN mkdir /Consumer /Domain /CrossCutting /Infrastructure /Application /Static

COPY ["./Consumer/appsettings.Docker.json", "./Infrastructure/"]

COPY ["./Consumer/Consumer.csproj", "./Consumer/"]
COPY ["./Domain/Domain.csproj", "./Domain/"]
COPY ["./CrossCutting/CrossCutting.csproj", "./CrossCutting/"]
COPY ["./Infrastructure/Infrastructure.csproj", "./Infrastructure/"]
COPY ["./Application/Application.csproj", "./Application/"]

COPY ["./Static/", "./Static/"]

# RESTORE
RUN dotnet restore Consumer
RUN dotnet restore Domain
RUN dotnet restore CrossCutting
RUN dotnet restore Infrastructure
RUN dotnet restore Application
COPY . .

# BUILD & TEST
RUN dotnet build "./Consumer/Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# RUN PUBLISH
FROM build AS publish

ARG BUILD_CONFIGURATION=Release
RUN dotnet publish /src/KafkaConsumer/Consumer/Consumer.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base as final

WORKDIR /app

COPY --from=publish /app/publish ./
COPY --from=build /src /src

ENTRYPOINT ["/bin/sh", "-c", "dotnet Consumer.dll"]