# Step09 · 集成测试 lab

对应：`步骤9-集成测试-完整实施指南.md`

本 lab 同时是：
1. 可运行的 Catalog API（EF + PostgreSQL + 启动自动迁移/种子）
2. 集成测试教学：`WebApplicationFactory` + 真实 PG（默认连 docker；测试可用 Testcontainers）

```bash
# docker PG 已运行即可
dotnet run --project src/Step09_IntegrationTesting
dotnet test tests/Step09_IntegrationTesting.Tests
```

为什么不用 EF InMemory：它不是真实 SQL 方言/约束/事务行为，会漏掉生产 bug。
