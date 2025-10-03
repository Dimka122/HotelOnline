FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HotelBooking.Web/HotelBooking.Web.csproj", "HotelBooking.Web/"]
COPY ["HotelBooking.Application/HotelBooking.Application.csproj", "HotelBooking.Application/"]
COPY ["HotelBooking.Domain/HotelBooking.Domain.csproj", "HotelBooking.Domain/"]
COPY ["HotelBooking.Infrastructure/HotelBooking.Infrastructure.csproj", "HotelBooking.Infrastructure/"]
RUN dotnet restore "HotelBooking.Web/HotelBooking.Web.csproj"
COPY . .
WORKDIR "/src/HotelBooking.Web"
RUN dotnet build "HotelBooking.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelBooking.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelBooking.Web.dll"]