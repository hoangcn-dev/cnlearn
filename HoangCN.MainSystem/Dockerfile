FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
# Copy các file dự án để cache lại bước restore
COPY ["HoangCN.MainSystem/HoangCN.MainSystem.csproj", "HoangCN.MainSystem/"]
COPY ["HoangCN.Common/HoangCN.Common.csproj", "HoangCN.Common/"]
COPY ["HoangCN.Core.BL/HoangCN.Core.BL.csproj", "HoangCN.Core.BL/"]
COPY ["HoangCN.Core.DL/HoangCN.Core.DL.csproj", "HoangCN.Core.DL/"]

RUN dotnet restore "HoangCN.MainSystem/HoangCN.MainSystem.csproj"

# Chỉ copy mã nguồn của các dự án phụ thuộc thực tế
COPY HoangCN.MainSystem/ HoangCN.MainSystem/
COPY HoangCN.Common/ HoangCN.Common/
COPY HoangCN.Core.BL/ HoangCN.Core.BL/
COPY HoangCN.Core.DL/ HoangCN.Core.DL/
RUN dotnet publish HoangCN.MainSystem/HoangCN.MainSystem.csproj -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HoangCN.MainSystem.dll"]
