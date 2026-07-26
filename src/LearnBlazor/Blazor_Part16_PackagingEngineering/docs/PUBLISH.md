# Part16 · 发布指南

## dotnet publish

```bash
dotnet publish Blazor_Part16_PackagingEngineering.csproj -c Release -o publish
```

Release 配置已在 csproj 中开启 `<Optimize>` 和 `<DebugType>none`（体积优先）。
要线上符号化，把 `<DebugType>` 改为 `portable`。

## Docker

```bash
docker build -t part16:latest .
docker run -p 8080:8080 part16:latest
```

多阶段构建：sdk 阶段 publish，aspnet runtime 阶段仅拷贝产物。

## 环境变量

- `ASPNETCORE_ENVIRONMENT`: `Production` / `Development`
- `ASPNETCORE_URLS`: 默认 `http://+:8080`（Dockerfile）

## 健康检查

- `GET /healthz` —— 接入负载均衡/编排系统。

## Trimming / AOT

Blazor Server 不推荐 `PublishTrimmed`（反射多）或 NativeAOT（SignalR/Emit 依赖）。
体积优化优先静态资产压缩、响应压缩、缓存头。

## 静态资产

`wwwroot/` 随程序集输出；Program.cs 给静态文件设了 `Cache-Control: public, max-age=3600`。
Blazor 框架脚本 `_framework/blazor.web.js` 带指纹文件名，可长期缓存。