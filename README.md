# ASP.NET Study Labs

**一文档 = 一独立项目**  
Worktree：`/home/sammiller/Project/LearnCSharp/.worktree/aspnet-study`  
分支：`feature/aspnet-study-labs`

## 怎么跑

```bash
cd /home/sammiller/Project/LearnCSharp/.worktree/aspnet-study

# 全部测试（推荐；WebApplicationFactory / Testcontainers，无需手写端口）
dotnet test AspNetStudyLabs.slnx

# 跑某个 lab（端口来自各项目 launchSettings.json）
dotnet run --project src/Step01_HostingAndStartup
dotnet run --project src/Part05_02_SpaBffAuth          # :5402 浏览器演示 BFF
dotnet run --project src/Part07_DistributedComms/Inventory
dotnet run --project src/Part07_DistributedComms/Catalog
dotnet run --project src/Part07_DistributedComms/Gateway
dotnet run --project src/Part10_Aspire/Api
# 可选完整 Aspire 编排（需 Docker）：
dotnet run --project src/Part10_Aspire/AppHost
```

Docker（`/home/sammiller/Project/docker`）已启动时默认连接：

| 服务 | 地址 |
|------|------|
| PostgreSQL | `localhost:5432` · `dotnet`/`dotnet_dev` |
| Redis | `localhost:6380` |
| RabbitMQ | `amqp://dotnet:dotnet_dev@localhost:5672/` |
| Seq | `http://localhost:5341` |
| Aspire Dashboard / OTLP | `18888` / `4317` |

---

## 测试结果（完整绿）

```text
dotnet test AspNetStudyLabs.slnx
# ~102 tests, 0 failed
```

### 本体 Step01–10（55）

| Lab | Tests |
|-----|------:|
| Step01 Hosting | 8 |
| Step02 DI/Options | 11 |
| Step03 Middleware | 6 |
| Step04 Routing | 6 |
| Step05 Minimal+Controller | 5 |
| Step06 Validation/ProblemDetails | 5 |
| Step07 JWT AuthZ | 4 |
| Step08 Logging/Health/Seq | 3 |
| Step09 Integration + PG | 5 |
| Step10 Kestrel/HttpClient | 2 |

### 第 3–10 部分（加深后）

| Lab | Tests | 完整度 |
|-----|------:|--------|
| Part03_01 API Design | 4 | 高 · 版本/keyset/ETag/幂等 |
| Part03_02 Architecture | 2 | 高 · 三模块 |
| Part03_03 Project Structure | 1 | 高 · 分层 |
| Part03_04 Architecture Tests | 3 | 高 · NetArchTest |
| Part04_01 EF Core | 3 | 高 · 真 PG/投影/N+1/并发 |
| Part04_02 Caching | 2 | 高 · Redis |
| Part04_03 MultiTenancy | 3 | 高 · 租户隔离 |
| Part05_01 Auth Core | 2 | 高 · JWT+限流+CORS |
| **Part05_02 SPA/BFF** | **6** | **高 · BFF cookie、CSRF、PKCE 说明、token 存储教育、演示页** |
| Part06_01 Message Patterns | 3 | 高 · Outbox |
| Part06_02 RabbitMQ | 1 | 高 · 真 RMQ |
| **Part07 Distributed** | **7** | **高 · Catalog+Inventory+Gateway、gRPC、弹性、YARP 鉴权、health** |
| Part08_01 OpenTelemetry | 2 | 高 · OTLP |
| **Part08_02 Troubleshooting** | **4** | **高 · 故障注入、checkout、metrics 指纹、CorrelationId** |
| Part09 Deployment | 1 | 高 · Dockerfile+/health |
| **Part10 Aspire** | **3** | **高 · ServiceDefaults+Api；AppHost 资源图可 `dotnet run`** |

---

## 加深 lab 要点

### Part05_02
- BFF：`HttpOnly` session cookie + **服务端 token 会话**
- CSRF：`X-CSRF: 1` + SameSite=Strict
- 教育 API：localStorage / memory / cookie 风险
- 静态演示页：`wwwroot/index.html`

### Part07
- **三个独立服务**：Gateway `5700` · Catalog `5701` · Inventory `5702`
- 对内 **gRPC**（`.proto`）+ 弹性 `AddStandardResilienceHandler`
- **YARP** 网关 + JWT 下沉
- live/ready health

### Part08_02
- `POST /diag/fault` 注入慢/失败  
- `GET /api/orders/checkout` 复现  
- `GET /diag/metrics` + Seq/Aspire 排障路径  

### Part10
- `ServiceDefaults`（OTel/Health/Resilience/Discovery）  
- `Api` 使用 defaults  
- `AppHost`：Postgres + Redis + RabbitMQ + Api 资源图  
