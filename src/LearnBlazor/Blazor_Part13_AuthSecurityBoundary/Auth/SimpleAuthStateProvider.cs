using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace LearnBlazor.Blazor_Part13_AuthSecurityBoundary.Auth;

public sealed class SimpleAuthStateProvider : AuthenticationStateProvider
{
    private AuthenticationState _state;

    public SimpleAuthStateProvider(IHttpContextAccessor http)
    {
        _state = new AuthenticationState(http.HttpContext?.User ?? new ClaimsPrincipal());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(_state);

    /// <summary>登录/登出后调用：刷新状态并通知 AuthorizeRouteView/AuthorizeView。</summary>
    public void NotifyChanged() => NotifyAuthenticationStateChanged(Task.FromResult(_state));
}