FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["hotel/RoomService/RoomService.csproj", "hotel/RoomService/"]
RUN dotnet restore "hotel/RoomService/RoomService.csproj"
COPY . .
WORKDIR "/src/hotel/RoomService"
RUN dotnet build "RoomService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoomService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoomService.dll"]