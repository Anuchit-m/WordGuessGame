FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WordGuessGame.csproj", "."]
RUN dotnet restore "WordGuessGame.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "WordGuessGame.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WordGuessGame.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# สร้าง /data directory และให้สิทธิ์ในการเขียน
RUN mkdir -p /data && chmod 777 /data

ENTRYPOINT ["dotnet", "WordGuessGame.dll"] 