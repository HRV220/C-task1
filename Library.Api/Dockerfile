FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Library.Domain/Library.Domain.csproj Library.Domain/
COPY Library.Application/Library.Application.csproj Library.Application/
COPY Library.Infrastructure/Library.Infrastructure.csproj Library.Infrastructure/
COPY Library.Api/Library.Api.csproj Library.Api/
RUN dotnet restore Library.Api/Library.Api.csproj
COPY Library.Domain/ Library.Domain/
COPY Library.Application/ Library.Application/
COPY Library.Infrastructure/ Library.Infrastructure/
COPY Library.Api/ Library.Api/
RUN dotnet publish Library.Api/Library.Api.csproj -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Library.Api.dll"]
