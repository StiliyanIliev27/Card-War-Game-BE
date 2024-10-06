# Use the official ASP.NET Core runtime image (Linux-based)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK image to build the app (Linux-based)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Card-War-Game-BE.csproj", "./"]
RUN dotnet restore "./Card-War-Game-BE.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Card-War-Game-BE.csproj" -c Release -o /app/build

# Publish the app to a folder for deployment
FROM build AS publish
RUN dotnet publish "Card-War-Game-BE.csproj" -c Release -o /app/publish

# Final stage - runtime image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Card-War-Game-BE.dll"]
