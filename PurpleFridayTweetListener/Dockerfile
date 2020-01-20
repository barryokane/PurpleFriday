FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

WORKDIR /app/PurpleFridayTweetListener.Logger
COPY PurpleFridayTweetListener.Logger/*.csproj .
RUN dotnet restore

# copy csproj and restore as distinct layers
WORKDIR /app/PurpleFridayTweetListener.Communicator
COPY PurpleFridayTweetListener.Communicator/*.csproj .
RUN dotnet restore

WORKDIR /app/PurpleFridayTweetListener.LocationFinder
COPY PurpleFridayTweetListener.LocationFinder/*.csproj .
RUN dotnet restore

WORKDIR /app/PurpleFridayTweetListener
COPY PurpleFridayTweetListener/*.csproj .
RUN dotnet restore

# copy everything else and build app
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/PurpleFridayTweetListener/out/ .
ENTRYPOINT ["dotnet", "PurpleFridayTweetListener.dll"]
