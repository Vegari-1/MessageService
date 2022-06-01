FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY MessageService/MessageService.csproj MessageService/
RUN dotnet restore MessageService/MessageService.csproj
COPY . .
WORKDIR /src/MessageService
RUN dotnet build MessageService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MessageService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MessageService.dll"]