using System.ComponentModel.DataAnnotations;

namespace LearnBlazor.Blazor_Part13_AuthSecurityBoundary.Models;

public sealed class LoginModel
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] [DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
}
