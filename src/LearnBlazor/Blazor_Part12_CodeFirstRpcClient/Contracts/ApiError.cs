namespace LearnBlazor.Blazor_Part12_CodeFirstRpcClient.Contracts;

/// <summary>
/// 把不可靠的网络失败收口成有限、可映射的领域错误，UI 永远不直接吃 RpcException/HttpRequestException。
/// </summary>
public enum ApiError
{
    None,
    Network,
    Timeout,
    NotFound,
    Server,
    Cancelled,
}