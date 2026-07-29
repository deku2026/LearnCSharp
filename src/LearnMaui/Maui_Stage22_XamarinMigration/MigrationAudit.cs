using System.Text.RegularExpressions;

namespace LearnMaui.Maui_Stage22_XamarinMigration;

public enum MigrationSeverity
{
    Blocker,
    Review,
}

public sealed record MigrationRule(
    string LegacyPattern,
    string ModernReplacement,
    MigrationSeverity Severity,
    string Rationale);

public sealed record MigrationFinding(
    int Line,
    string LegacyPattern,
    string ModernReplacement,
    MigrationSeverity Severity);

public sealed class MigrationAuditor
{
    private static readonly MigrationRule[] Rules =
    [
        new(@"Xamarin\.Forms", "Microsoft.Maui.Controls", MigrationSeverity.Blocker, "Old UI namespace"),
        new(@"\bRenderer\b|ExportRenderer", "Handler + mapper", MigrationSeverity.Blocker, "Renderer lifecycle is incompatible"),
        new(@"\bDependencyService\b", "Constructor injection", MigrationSeverity.Blocker, "Service locator hides dependencies"),
        new(@"\bMessagingCenter\b", "WeakReferenceMessenger or typed event", MigrationSeverity.Blocker, "Global string bus leaks ownership"),
        new(@"\bListView\b|\bTableView\b|\bViewCell\b|\bTextCell\b", "CollectionView + DataTemplate", MigrationSeverity.Blocker, "Legacy cell model"),
        new(@"\bFrame\b", "Border", MigrationSeverity.Review, "Border has the modern stroke/shape model"),
        new(@"\bClickGestureRecognizer\b", "TapGestureRecognizer", MigrationSeverity.Blocker, "Legacy gesture"),
        new(@"Xamarin\.Essentials", "Microsoft.Maui.* Essentials APIs", MigrationSeverity.Blocker, "Essentials is integrated"),
        new(@"Android\.Support\.", "AndroidX", MigrationSeverity.Blocker, "Support libraries are obsolete"),
        new(@"\bDevice\.BeginInvokeOnMainThread\b", "Dispatcher.Dispatch", MigrationSeverity.Review, "Use an owned dispatcher"),
        new(@"\bDisplayAlert\s*\(", "DisplayAlertAsync", MigrationSeverity.Review, "Prefer async API"),
        new(@"\bFadeTo\s*\(", "FadeToAsync", MigrationSeverity.Review, "Prefer async animation API"),
        new(@"\bDevice\.RuntimePlatform\b", "DeviceInfo.Platform or compile-time service", MigrationSeverity.Review, "Keep platform branching at the edge"),
    ];

    public IReadOnlyList<MigrationRule> Catalog => Rules;

    public IReadOnlyList<MigrationFinding> Scan(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        List<MigrationFinding> findings = [];
        string[] lines = source.ReplaceLineEndings("\n").Split('\n');
        for (int index = 0; index < lines.Length; index++)
        {
            foreach (MigrationRule rule in Rules)
            {
                if (Regex.IsMatch(
                    lines[index],
                    rule.LegacyPattern,
                    RegexOptions.CultureInvariant,
                    TimeSpan.FromMilliseconds(100)))
                {
                    findings.Add(new MigrationFinding(
                        index + 1,
                        rule.LegacyPattern,
                        rule.ModernReplacement,
                        rule.Severity));
                }
            }
        }

        return findings;
    }
}

public enum MigrationWave
{
    ProjectSkeleton,
    StartupAndDependencyInjection,
    NavigationShell,
    ForbiddenControls,
    HandlersAndEffects,
    EssentialsAndPlugins,
    MessagingAndArchitecture,
    ReleaseQuality,
}

public sealed record MigrationBatch(
    MigrationWave Wave,
    string Scope,
    string ExitEvidence,
    bool RollbackReady);

public static class MigrationPlanner
{
    public static IReadOnlyList<MigrationBatch> CreateDefaultPlan() =>
    [
        new(MigrationWave.ProjectSkeleton, "SDK-style single project and resources", "Android app starts", true),
        new(MigrationWave.StartupAndDependencyInjection, "MauiProgram composition root", "No service locator", true),
        new(MigrationWave.NavigationShell, "Routes and navigation parameters", "Deep link/back regression", true),
        new(MigrationWave.ForbiddenControls, "Page-sized UI slices", "Visual + accessibility snapshot", true),
        new(MigrationWave.HandlersAndEffects, "One native customization at a time", "Connect/disconnect evidence", true),
        new(MigrationWave.EssentialsAndPlugins, "Permissions, files, secure storage", "Denied/cancelled cases", true),
        new(MigrationWave.MessagingAndArchitecture, "Typed ownership boundaries", "Subscriber can be collected", true),
        new(MigrationWave.ReleaseQuality, "Trim, signing and store package", "Installed release smoke", true),
    ];
}

public sealed record CompatibilityShim(
    string Name,
    DateOnly Introduced,
    DateOnly RemoveBy,
    string Owner)
{
    public bool IsExpired(DateOnly today) => today > RemoveBy;
}

public static class ModernMigrationSlice
{
    public static View Create()
    {
        Label label = new()
        {
            Text = "MAUI-native migration slice",
            AutomationId = "MigrationSlice.Title",
        };
        TapGestureRecognizer tap = new();
        tap.Tapped += (_, _) => label.Text = "TapGestureRecognizer verified";
        label.GestureRecognizers.Add(tap);

        return new Border
        {
            Stroke = Colors.CornflowerBlue,
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 12,
            },
            Padding = 16,
            Content = label,
        };
    }
}
