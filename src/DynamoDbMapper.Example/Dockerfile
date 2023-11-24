FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DynamoDbMapper.Example/DynamoDbMapper.Example.csproj", "DynamoDbMapper.Example/"]
RUN dotnet restore "DynamoDbMapper.Example/DynamoDbMapper.Example.csproj"
COPY . .
WORKDIR "/src/DynamoDbMapper.Example"
RUN dotnet build "DynamoDbMapper.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DynamoDbMapper.Example.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamoDbMapper.Example.dll"]
