using System.ComponentModel.DataAnnotations;

namespace LearnBlazor.Blazor_Part06_FormsInputTouch.Models;

public sealed class Contact
{
    [Required(ErrorMessage = "姓名必填")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "姓名至少 2 字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱必填")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "留言不超过 500 字符")]
    public string Message { get; set; } = string.Empty;

    public ContactCategory Category { get; set; } = ContactCategory.General;

    public bool Agree { get; set; }
}

public enum ContactCategory { General, Sales, Support }