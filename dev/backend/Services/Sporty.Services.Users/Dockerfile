#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Sporty.Services.Users/Sporty.Services.Users.csproj", "Services/Sporty.Services.Users/"]
COPY ["Infra/Sporty.Infra.Data.Accessor/Sporty.Infra.Data.Accessor.csproj", "Infra/Sporty.Infra.Data.Accessor/"]
COPY ["Common/Sporty.Common.Dto/Sporty.Common.Dto.csproj", "Common/Sporty.Common.Dto/"]
COPY ["Infra/Sporty.Infra.WebApi/Sporty.Infra.WebApi.csproj", "Infra/Sporty.Infra.WebApi/"]
COPY ["Common/Sporty.Common.Network/Sporty.Common.Network.csproj", "Common/Sporty.Common.Network/"]
RUN dotnet restore "Services/Sporty.Services.Users/Sporty.Services.Users.csproj"
COPY . .
WORKDIR "/src/Services/Sporty.Services.Users"
RUN dotnet build "Sporty.Services.Users.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sporty.Services.Users.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sporty.Services.Users.dll"]