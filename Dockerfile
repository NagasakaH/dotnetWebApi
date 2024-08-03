FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder
WORKDIR /build
COPY src/*.csproj ./
RUN dotnet restore
COPY /src ./
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "WebApi.dll"]