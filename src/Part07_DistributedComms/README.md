# Part07 · 分布式与微服务通信

对应：`第7部分-分布式与微服务通信-完整实施指南.md`

## 三个独立进程

| 服务 | 端口 | 职责 |
|------|------|------|
| Gateway (YARP) | 5700 | 统一入口 + JWT 下沉 |
| Catalog | 5701 | 下单编排，gRPC 调库存 |
| Inventory | 5702 | gRPC + REST 库存 |

```bash
# 三个终端（端口来自 launchSettings，无需手写 --urls）
dotnet run --project src/Part07_DistributedComms/Inventory
dotnet run --project src/Part07_DistributedComms/Catalog
dotnet run --project src/Part07_DistributedComms/Gateway
```

## 覆盖

- 服务拆分（Catalog / Inventory / Gateway）
- 对内 **gRPC**（proto contract-first）
- **HttpClient + AddStandardResilienceHandler**
- **YARP** 路由 + 网关鉴权
- `/health/live` + `/health/ready`
- Correlation-Id 透传
