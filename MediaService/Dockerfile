FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["hotel/MediaService/MediaService.csproj", "hotel/MediaService/"]
RUN dotnet restore "hotel/MediaService/MediaService.csproj"
COPY . .
WORKDIR "/src/hotel/MediaService"
RUN dotnet build "MediaService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediaService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.dll"]