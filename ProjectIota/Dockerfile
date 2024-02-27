#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

#FastReport
RUN ln -s /lib/x86_64-linux-gnu/libdl-2.24.so /lib/x86_64-linux-gnu/libdl.so
RUN apt-get update \
 && apt-get install -y --allow-unauthenticated \
 libc6-dev \
 libgdiplus \
 libx11-dev \
 && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ProjectIota/ProjectIota.csproj", "ProjectIota/"]
RUN dotnet restore "ProjectIota/ProjectIota.csproj"
COPY . .
WORKDIR "/src/ProjectIota"
RUN dotnet build "ProjectIota.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectIota.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectIota.dll"]