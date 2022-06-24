FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["hotel/main/main.csproj", "hotel/main/"]
RUN dotnet restore "hotel/main/main.csproj"
COPY . .
WORKDIR "/src/hotel/main"
RUN dotnet build "main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "main.dll"]