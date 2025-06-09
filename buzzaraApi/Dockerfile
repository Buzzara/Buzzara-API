                                                                                                                           # Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build;
WORKDIR /app;
COPY . .
RUN dotnet publish -c Release -o /publish;

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0;
WORKDIR /app;
COPY --from=build /publish .
EXPOSE 8080;
ENTRYPOINT ["dotnet", "buzzaraApi.dll"];
