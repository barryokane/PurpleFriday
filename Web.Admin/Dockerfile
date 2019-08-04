FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
WORKDIR /app/Web.Admin
COPY *.csproj .
RUN dotnet restore

# copy everything else and build app
WORKDIR /app
COPY . ./
COPY _datastore ./
#RUN dotnet publish -c Release -o out
RUN dotnet publish -c Debug -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Web.Admin.dll"]
