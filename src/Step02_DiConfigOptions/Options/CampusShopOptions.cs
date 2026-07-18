using System.ComponentModel.DataAnnotations;

namespace Step02_DiConfigOptions.Options;

/// <summary>Strongly-typed options for campus shop SMTP (demo).</summary>
public sealed class CampusShopOptions
{
    public const string SectionName = "CampusShop";

    [Required]
    [MinLength(1)]
    public string ShopName { get; set; } = "";

    [Required]
    public string SmtpHost { get; set; } = "";

    [Range(1, 65535)]
    public int SmtpPort { get; set; }

    public bool EnablePromotions { get; set; }
}
