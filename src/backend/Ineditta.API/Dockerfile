#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update -qq && apt-get -y install locales libgdiplus libc6-dev wkhtmltopdf
RUN ln -s /usr/lib/libgdiplus.so/usr/lib/gdiplus.dll

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/backend/Ineditta.API/Ineditta.API.csproj", "src/backend/Ineditta.API/"]
COPY ["src/backend/Ineditta.Application/Ineditta.Application.csproj", "src/backend/Ineditta.Application/"]
COPY ["src/backend/Ineditta.BuildingBlocks.Core/Ineditta.BuildingBlocks.Core.csproj", "src/backend/Ineditta.BuildingBlocks.Core/"]
COPY ["src/backend/Ineditta.BuildingBlocks.BackgroundProcessing/Ineditta.BuildingBlocks.BackgroundProcessing.csproj", "src/backend/Ineditta.BuildingBlocks.BackgroundProcessing/"]
COPY ["src/backend/Ineditta.Logs/Ineditta.Logs.csproj", "src/backend/Ineditta.Logs/"]
COPY ["src/backend/Ineditta.Repository/Ineditta.Repository.csproj", "src/backend/Ineditta.Repository/"]
COPY ["src/backend/Ineditta.Integration/Ineditta.Integration.csproj", "src/backend/Ineditta.Integration/"]
RUN dotnet restore "src/backend/Ineditta.API/Ineditta.API.csproj"
COPY . .
WORKDIR "/src/src/backend/Ineditta.API"
RUN dotnet build "Ineditta.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ineditta.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ineditta.API.dll"]