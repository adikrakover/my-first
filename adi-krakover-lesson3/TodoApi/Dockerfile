
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    COPY ["TodoApi/TodoApi.csproj", "TodoApi/"]
    RUN dotnet restore "TodoApi/TodoApi.csproj"
    

    COPY . .
    
    WORKDIR /src/TodoApi
    RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish
    

    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
    WORKDIR /app
    
    EXPOSE 8080
    
    COPY --from=build /app/publish .
    
    ENTRYPOINT ["dotnet", "TodoApi.dll"]