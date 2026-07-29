using System.Net;
using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage11_NetworkDataAuth;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "HTTP 方法与幂等",
                "GET/HEAD 安全且幂等，PUT/DELETE 通常幂等，POST 通常不是。重试策略必须理解业务幂等键，不能看到异常就重复提交。",
                "ArticlesClient uses GET for page reads"),
            new(LearningLevel.Beginner, "状态码分层",
                "2xx 成功、401 认证、404 缺失、4xx 校验/业务、429/5xx 临时失败分别映射成可处理结果，不把所有非 200 都抛成同一错误。",
                "NetworkOutcome mapping"),
            new(LearningLevel.Beginner, "HttpClient 生命周期",
                "复用 HttpClient/handler 连接池；不要每请求 new+Dispose。客户端设置合理 BaseAddress、Accept 和总超时，每请求另带 Token。",
                "AddHttpClient<IArticlesClient, ArticlesClient>"),
            new(LearningLevel.Beginner, "Typed Client",
                "接口返回领域结果而非裸 HttpResponseMessage，客户端负责 URI、header、状态码、JSON 与 DTO→Domain 映射。",
                "IArticlesClient + ArticlesClient"),
            new(LearningLevel.Beginner, "取消与超时",
                "用户/页面取消与每次请求超时分开；Token 贯穿 SendAsync、ReadAsStreamAsync、DeserializeAsync。取消不是网络故障。",
                "GetPageAsync(CancellationToken)"),
            new(LearningLevel.Intermediate, "IHttpClientFactory 客户端边界",
                "Factory 管 handler 轮换与 DI，但移动客户端仍需理解离线、应用休眠和 cookie/token 生命周期；不要照搬服务端每请求 Scope 心智。",
                "MauiProgram AddHttpClient + DelegatingHandler"),
            new(LearningLevel.Intermediate, "流式响应",
                "ResponseHeadersRead 先拿 header，再从 Stream 增量读取，降低大响应内存；下载写临时文件后原子替换并报告进度。",
                "HttpCompletionOption.ResponseHeadersRead"),
            new(LearningLevel.Intermediate, "JSON Source Generation",
                "JsonSerializerContext 生成 DTO 元数据，避免运行时反射并改善 trim/AOT；未知字段允许向前兼容，关键缺失字段仍需校验。",
                "ApiJsonContext"),
            new(LearningLevel.Intermediate, "DTO 与 Domain 分离",
                "线上的 nullable/命名/版本 DTO 不直接成为业务模型；映射层建立不变量，Domain 不携带 JsonPropertyName。",
                "ArticleDto → Article"),
            new(LearningLevel.Intermediate, "重试、退避与抖动",
                "只对明确瞬时且可安全重放的操作重试，尊重 Retry-After，指数退避加 jitter，并受页面取消和总预算约束。",
                "NetworkOutcome.TransientFailure is retry-policy input"),
            new(LearningLevel.Intermediate, "熔断与请求去重",
                "连续失败短期开路保护电量/服务；相同查询 single-flight 共用 Task，但调用者取消语义与缓存失效必须定义。",
                "SemaphoreSlim pattern reused by TokenRefreshCoordinator"),
            new(LearningLevel.Advanced, "弱网与离线",
                "Connectivity 只是提示，真实请求仍可能失败；UI 显示缓存内容+过期标识，重连触发有限同步，不循环弹窗。",
                "HTTP result, not Connectivity, is final truth"),
            new(LearningLevel.Advanced, "Cursor 分页",
                "Cursor 对动态列表比 page number 稳定；VM 保存 NextCursor、HasMore 与 IsLoadingMore，防重复触底并去重 item id。",
                "ArticlePage.NextCursor"),
            new(LearningLevel.Advanced, "上传下载",
                "上传用 StreamContent/Multipart，避免整文件进内存；下载逐块读取、计算进度、校验长度/哈希并支持取消。",
                "stream-first design represented by response reading"),
            new(LearningLevel.Advanced, "证书与 Pinning",
                "默认信任系统根和现代 TLS。Pinning 需要双 key 轮换、过期、应急和遥测，是运营系统，不是一行回调接受证书。",
                "no permissive certificate callback"),
            new(LearningLevel.Advanced, "OAuth/OIDC + PKCE",
                "移动公共客户端使用 Authorization Code + PKCE、state/nonce、系统浏览器和精确回调 URI；不内嵌 client secret。",
                "PkcePair.Create uses CSPRNG + SHA-256"),
            new(LearningLevel.Advanced, "令牌安全存储",
                "access/refresh token 放 SecureStorage 抽象，日志永不输出 token。登出清本地令牌、cookie/账户状态，并按协议撤销可撤销凭据。",
                "ITokenVault abstraction; MemoryTokenVault only for lab"),
            new(LearningLevel.Expert, "Auth Header 与单飞刷新",
                "DelegatingHandler 在请求前取有效 token；过期刷新由 SemaphoreSlim single-flight，二次检查避免并发风暴。401 只能安全重放一次。",
                "AuthHeaderHandler + TokenRefreshCoordinator"),
            new(LearningLevel.Expert, "日志红action",
                "记录方法、host、route template、状态码、耗时和 correlation id；删除 Authorization、cookie、query token、PII 与响应正文。",
                "typed diagnostics exclude credentials"),
            new(LearningLevel.Expert, "测试矩阵",
                "Stub handler 覆盖 success/401/404/429/5xx/坏 JSON/慢流/取消；集成环境验证契约，真机验证代理、休眠、弱网和认证回调。",
                "StubHttpHandler"),
        ];

        LearningLab[] labs =
        [
            new("解析分页 JSON", "通过 StubHttpHandler、流式读取和 SourceGen Context 完成 DTO→Domain。",
                async token =>
                {
                    string json =
                        """
                        {"items":[{"id":"00000000-0000-0000-0000-000000000011","title":"MAUI network","updatedAt":"2026-07-30T00:00:00Z"}],"nextCursor":"next-1","futureField":true}
                        """;
                    StubHttpHandler handler = new(HttpStatusCode.OK, json);
                    HttpClient httpClient = new(handler)
                    {
                        BaseAddress = new Uri("https://test.example/"),
                    };
                    ArticlesClient client = new(httpClient);
                    NetworkResult<ArticlePage> result =
                        await client.GetPageAsync(null, token);
                    return $"{result.Outcome}, count={result.Value?.Items.Count}, cursor={result.Value?.NextCursor}";
                }),
            new("映射服务器失败", "把 503 映射为 TransientFailure，供退避/熔断策略判断。",
                async token =>
                {
                    HttpClient httpClient = new(
                        new StubHttpHandler(HttpStatusCode.ServiceUnavailable, "{}"))
                    {
                        BaseAddress = new Uri("https://test.example/"),
                    };
                    NetworkResult<ArticlePage> result =
                        await new ArticlesClient(httpClient).GetPageAsync(null, token);
                    return $"{result.Outcome}, status={(int?)result.StatusCode}";
                }),
            new("生成 PKCE", "生成高熵 verifier 与 SHA-256 base64url challenge。",
                _ =>
                {
                    PkcePair pair = PkcePair.Create();
                    return Task.FromResult(
                        $"verifier length={pair.Verifier.Length}\nchallenge length={pair.Challenge.Length}\nsecret logged=false");
                }),
            new("验证单飞刷新", "并发请求一个即将过期 token，确认最终共享同一刷新结果。",
                async token =>
                {
                    MemoryTokenVault vault = new();
                    await vault.SetAsync(
                        new TokenSnapshot("old", "refresh", DateTimeOffset.UtcNow),
                        token);
                    TokenRefreshCoordinator coordinator = new(vault);
                    TokenSnapshot?[] tokens = await Task.WhenAll(
                        coordinator.GetValidTokenAsync(token),
                        coordinator.GetValidTokenAsync(token),
                        coordinator.GetValidTokenAsync(token));
                    int distinct = tokens
                        .Select(value => value?.AccessToken)
                        .Distinct()
                        .Count();
                    return $"并发调用=3，最终 access token 数={distinct}";
                }),
            new("验证 cursor 编码", "分页 cursor 作为 query 组件转义，避免注入额外参数。",
                async token =>
                {
                    StubHttpHandler handler = new(HttpStatusCode.OK, """{"items":[],"nextCursor":null}""");
                    HttpClient httpClient = new(handler)
                    {
                        BaseAddress = new Uri("https://test.example/"),
                    };
                    await new ArticlesClient(httpClient)
                        .GetPageAsync("a&admin=true", token);
                    return handler.LastRequest?.RequestUri?.ToString() ?? "no request";
                }),
        ];

        return new LearningStage(
            11,
            "网络、数据与认证",
            "第5部分-阶段11-网络数据与认证.md",
            topics,
            labs);
    }
}
