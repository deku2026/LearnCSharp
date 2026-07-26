# syntax=dockerfile
# Part16 - 多阶段构建：sdk 构建 → aspnet 运行
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 先拷贝 csproj 还原（利用层缓存）
COPY Blazor_Part16_PackagingEngineering.csproj ./
RUN dotnet restore

# 再拷贝源码发布
COPY . ./
RUN dotnet publish Blazor_Part16_PackagingEngineering.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Blazor_Part16_PackagingEngineering.dll"]