# Part05_02 · 前后端分离 / SPA 认证（BFF）

对应文档：`第5部分-2-前后端分离SPA认证-完整实施指南.md`

## 运行

```bash
dotnet run --project src/Part05_02_SpaBffAuth
# http://localhost:5402/  打开演示 SPA
```

## 覆盖点

- Token 三种存储风险教育 API：`GET /api/education/token-storage`
- 模拟 PKCE 换 token：`POST /idp/token`
- **BFF**：`/bff/login|logout|me|api/orders` — token 仅服务端会话，浏览器 HttpOnly cookie
- **CSRF**：BFF 写操作需 `X-CSRF: 1` + SameSite=Strict
- 反面教材：`POST /spa/insecure-demo/login` 把 token 返回给 JS

账号：`student/campus123` · `admin/campus123`
