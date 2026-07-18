# Step02 · 依赖注入 + 配置 + Options

## 对应文档

`/home/sammiller/Project/ArchitectureDesign/ASP.NetStudy/步骤2-依赖注入-配置-Options-完整实施指南.md`

## 运行

```bash
dotnet run --project src/Step02_DiConfigOptions --launch-profile http
# http://localhost:5102/di/lifetimes
# http://localhost:5102/options/shop
```

开发机密示例：

```bash
cd src/Step02_DiConfigOptions
dotnet user-secrets set "Greeting" "from-user-secrets"
dotnet run
# /config/greeting 应看到 from-user-secrets（Dev 环境）
```

启动期 Options 校验（故意失败）：

```bash
# 将 appsettings 中 SmtpPort 改为 0 后 dotnet run → ValidateOnStart 失败
```

## 测试

```bash
dotnet test tests/Step02_DiConfigOptions.Tests
```

## 验收对照

| 验收项 | 落点 |
|--------|------|
| 三生命周期 Id 证明 | `GET /di/lifetimes` + 测试 |
| 俘获依赖 / ValidateScopes | README 说明 + Hosted 用 ScopeFactory |
| FakeDbContext scoped + 后台 ScopeFactory | `ScopedSafeWorker` |
| IEnumerable 多实现 | `GET /di/writers` |
| Keyed services | `GET /di/pay/{alipay\|wechat}` |
| Scrutor 装饰器 | `GET /di/students` |
| 配置优先级 | appsettings 分层 + user-secrets 说明 |
| IOptions / Snapshot / Monitor | 三个 `/options/*` 端点 |
| ValidateOnStart | 无效配置时启动失败测试 |
