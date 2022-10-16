FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS  build
WORKDIR /src
COPY ["Cloud.$ext_safeprojectname$.API/Cloud.$ext_safeprojectname$.API.csproj", "Cloud.$ext_safeprojectname$.API/"]
COPY ["Cloud.$ext_safeprojectname$.Domain/Cloud.$ext_safeprojectname$.Domain.csproj", "Cloud.$ext_safeprojectname$.Domain/"]
COPY ["Cloud.$ext_safeprojectname$.Events/Cloud.$ext_safeprojectname$.Events.csproj", "Cloud.$ext_safeprojectname$.Events/"]
COPY ["Cloud.$ext_safeprojectname$.Infrastructure/Cloud.$ext_safeprojectname$.Infrastructure.csproj", "Cloud.$ext_safeprojectname$.Infrastructure/"]


RUN dotnet restore "Cloud.$ext_safeprojectname$.API/Cloud.$ext_safeprojectname$.API.csproj"
COPY . .
WORKDIR "/src/Cloud.$ext_safeprojectname$.API"
RUN dotnet build "Cloud.$ext_safeprojectname$.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cloud.$ext_safeprojectname$.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cloud.$ext_safeprojectname$.Api.dll"]