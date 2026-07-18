# Step08 · 日志 / 错误处理 / 健康检查

对应：`步骤8-日志-错误处理-健康检查-完整实施指南.md`

依赖已启动的 docker：`Seq`、`Postgres`、`Redis`（`/home/sammiller/Project/docker`）。

```bash
dotnet run --project src/Step08_LoggingErrorsHealth
# /health/live  /health/ready  Seq UI http://localhost:5341
```

配置见 `appsettings.json`，**无需手动传端口**（launchSettings 默认 5108）。
