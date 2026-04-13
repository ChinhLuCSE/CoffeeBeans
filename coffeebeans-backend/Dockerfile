# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy solution-level files first
COPY ["CoffeeBeans.backend.slnx", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]

# copy project files
COPY ["src/CoffeeBeans.backend.Web/CoffeeBeans.backend.Web.csproj", "src/CoffeeBeans.backend.Web/"]

RUN dotnet restore "src/CoffeeBeans.backend.Web/CoffeeBeans.backend.Web.csproj"

# copy the rest
COPY . .

WORKDIR /src/src/CoffeeBeans.backend.Web
RUN dotnet publish "CoffeeBeans.backend.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CoffeeBeans.backend.Web.dll"]