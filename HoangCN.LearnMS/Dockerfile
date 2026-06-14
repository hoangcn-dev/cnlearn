FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
# Copy project files for caching restore
COPY ["HoangCN.LearnMS/HoangCN.LearnMS.csproj", "HoangCN.LearnMS/"]
COPY ["HoangCN.Core.Common/HoangCN.Core.Common.csproj", "HoangCN.Core.Common/"]
COPY ["HoangCN.Core.BL/HoangCN.Core.BL.csproj", "HoangCN.Core.BL/"]
COPY ["HoangCN.Core.DL/HoangCN.Core.DL.csproj", "HoangCN.Core.DL/"]

RUN dotnet restore "HoangCN.LearnMS/HoangCN.LearnMS.csproj"

# Copy source code
COPY HoangCN.LearnMS/ HoangCN.LearnMS/
COPY HoangCN.Core.Common/ HoangCN.Core.Common/
COPY HoangCN.Core.BL/ HoangCN.Core.BL/
COPY HoangCN.Core.DL/ HoangCN.Core.DL/
RUN dotnet publish HoangCN.LearnMS/HoangCN.LearnMS.csproj -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HoangCN.LearnMS.dll"]
