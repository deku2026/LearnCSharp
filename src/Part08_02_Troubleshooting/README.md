# Part08_02 · 生产排障流程

对应：`第8部分-2-生产排障流程-完整实施指南.md`

## 运行

```bash
dotnet run --project src/Part08_02_Troubleshooting
# launchSettings → :5703，并指向 Seq/OTLP
```

## 6 步演练

1. `POST /diag/fault` `{"delayMs":800,"failCount":2}` 注入慢/失败  
2. `GET /api/orders/checkout` 复现  
3. `GET /diag/metrics` 看 avg/max、fail 计数  
4. Aspire Dashboard `http://localhost:18888` 看 trace  
5. Seq `http://localhost:5341` 按 CorrelationId 过滤  
6. 对照 fingerprintHints 写根因  

依赖 docker：Seq + Aspire Dashboard（OTLP）。
