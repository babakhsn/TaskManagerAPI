# ====== build ======
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the csproj files first (layered restore)
COPY src/TaskManager.Api/TaskManager.Api.csproj src/TaskManager.Api/
COPY src/TaskManager.Application/TaskManager.Application.csproj src/TaskManager.Application/
COPY src/TaskManager.Domain/TaskManager.Domain.csproj src/TaskManager.Domain/
COPY src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj src/TaskManager.Infrastructure/

# Restore the API project (this pulls transitive refs to Application/Domain/Infrastructure)
RUN dotnet restore src/TaskManager.Api/TaskManager.Api.csproj

# Now copy the rest of the source
COPY . .

# Publish the API
RUN dotnet publish src/TaskManager.Api/TaskManager.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ====== runtime ======
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]
