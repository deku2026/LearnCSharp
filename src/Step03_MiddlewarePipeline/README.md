# Step03 · 中间件管道

对应文档：`步骤3-中间件管道-完整实施指南.md`

```bash
dotnet run --project src/Step03_MiddlewarePipeline
# 端口见 Properties/launchSettings.json（默认 5103），无需手写 --urls
```

验收：计时中间件（约定式 + IMiddleware）、全局异常、短路(X-Api-Key)、MapShortCircuit、UseWhen、OnStarting、顺序铁律说明。
