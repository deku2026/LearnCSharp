# Step01 · 承载与启动模型 (Host / WebApplication)

## 对应文档

`/home/sammiller/Project/ArchitectureDesign/ASP.NetStudy/步骤1-承载与启动模型-完整实施指南.md`

## 本步目标

不靠“神秘模板行为”，写清并跑通：

`CreateBuilder` → `Build` → 配管道 → `Run`

并覆盖环境分层、内容根/Web 根、`BackgroundService` 优雅启停、生命周期事件。

## 运行

```bash
cd src/Step01_HostingAndStartup
dotnet run --launch-profile http
# http://localhost:5101/
# http://localhost:5101/host-info
# http://localhost:5101/readme.txt   (wwwroot 静态文件)
```

环境切换（验证配置分层）：

```bash
dotnet run --no-launch-profile --environment Production --urls http://localhost:5103
dotnet run --launch-profile staging-local
```

## 测试

```bash
dotnet test tests/Step01_HostingAndStartup.Tests
```

## 验收对照（文档第四节）

| 验收项 | 本项目落点 |
|--------|------------|
| 手写最小 Program 四阶段 | `Program.cs` |
| Builder 六装配点 + Build 默认装配 | 注释 + 代码使用 Services/Config/Logging/Environment/Host(HostOptions)/WebHost |
| WebApplication 三合一；Run vs Start | README 本节下方说明 + 使用 `Run` |
| 环境差异、launchSettings 不部署/不放密钥 | `appsettings.*.json` + `Properties/launchSettings.json` |
| 内容根 vs Web 根 | `/` 与 `/host-info` 返回路径；`wwwroot/readme.txt` |
| BackgroundService + PeriodicTimer + stoppingToken + 异常兜底 + IServiceScopeFactory | `CampusHeartbeatWorker` |
| Lifetime 事件 | `LifetimeEventsLogger` |
| 🔷 GenericWebHostService | 见下方专家笔记 |

### Run vs Start

- **`Run` / `RunAsync`**：启动后**阻塞**直到关闭（Web/控制台默认）。
- **`Start` / `StartAsync`**：启动后**立即返回**（需要启动后继续执行其他逻辑时）。

### 环境变量优先级（WebApplication）

使用 `WebApplication` 时：**`DOTNET_ENVIRONMENT` 优先于 `ASPNETCORE_ENVIRONMENT`**。  
`launchSettings.json` 仅本地开发注入环境变量，**不随 publish 部署**。

### 🔷 专家笔记

Web 服务器是主机里**众多 `IHostedService` 之一**：`GenericWebHostService` 负责启动 Kestrel。  
源码：`dotnet/aspnetcore` → `src/DefaultBuilder`、`src/Hosting`。

## 故意不做（留给后续步骤）

- 完整 DI 生命周期深挖 → 步骤 2  
- 中间件管道顺序 → 步骤 3  
- 生产级 Serilog/Health → 步骤 8  
- EF / 真数据库 → 第 4 部分  
