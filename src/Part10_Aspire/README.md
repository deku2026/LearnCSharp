# Part10 · .NET Aspire（克制定位）

对应：`第10部分-Aspire-完整实施指南.md`

## 结构

| 项目 | 作用 |
|------|------|
| `ServiceDefaults` | OTel + Health + Http resilience + ServiceDiscovery |
| `Api` | 示例服务（使用 ServiceDefaults） |
| `AppHost` | C# 资源图：API + Postgres + Redis + RabbitMQ |

## 运行

```bash
# 仅 API（对接已启动的 docker OTLP Dashboard）
dotnet run --project src/Part10_Aspire/Api

# 完整 AppHost（需 Docker；Aspire 会起依赖容器）
dotnet run --project src/Part10_Aspire/AppHost
```

## 克制认知

- Aspire = **开发期编排**，不是生产运行时  
- 不替代 YARP / 消息框架 / Outbox-Saga / 身份系统  
- Dashboard：`http://localhost:18888`（或 Aspire 自带 Dashboard）
