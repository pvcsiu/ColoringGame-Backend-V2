# 1. Dùng image SDK của Microsoft để build code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copy file cấu hình và tải các thư viện
COPY ["ColoringGame.API.csproj", "./"]
RUN dotnet restore "ColoringGame.API.csproj"

# 3. Copy toàn bộ code và tiến hành đóng gói (Publish)
COPY . .
RUN dotnet publish "ColoringGame.API.csproj" -c Release -o /app/publish

# 4. Chuyển sang image Runtime siêu nhẹ để chạy thực tế
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# ĐÃ SỬA LỖI Ở DÒNG NÀY: Copy từ bước 'build' thay vì 'publish'
COPY --from=build /app/publish .

# 5. Cấu hình cổng mạng cho Render
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 6. Khởi động Server
ENTRYPOINT ["dotnet", "ColoringGame.API.dll"]