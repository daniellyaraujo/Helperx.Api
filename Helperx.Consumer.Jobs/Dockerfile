ENTRYPOINT ["dotnet", "Helperx.Consumer.Jobs.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Helperx.Consumer.Jobs.csproj"
RUN dotnet publish "Helperx.Consumer.Jobs.csproj" -c Release -o /app/publish

ENV ConnectionStrings__DefaultConnection="Server=(LocalDb)\\MSSQLLocalDB;Database=DB;Integrated Security=true;"

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Helperx.Consumer.Jobs.dll"]