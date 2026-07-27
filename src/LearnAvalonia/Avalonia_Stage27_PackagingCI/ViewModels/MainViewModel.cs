using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§0 导读：打包、部署、更新与 CI", "第10部分-阶段27-打包部署更新与CI.md §0", "口述打包三目标", () => new OverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§1 Windows", "第10部分-阶段27-打包部署更新与CI.md §1", "MSIX/MSI 命令可演示", () => new WindowsPackDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2 macOS", "第10部分-阶段27-打包部署更新与CI.md §2", ".app/.dmg/签名/公证", () => new MacOsPackDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§3 Linux 桌面", "第10部分-阶段27-打包部署更新与CI.md §3", "AppImage/Flatpak/deb/rpm", () => new LinuxPackDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§4 Android", "第10部分-阶段27-打包部署更新与CI.md §4", "AAB/APK + 签名", () => new AndroidPackDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§5 iOS / iPadOS", "第10部分-阶段27-打包部署更新与CI.md §5", "IPA + 签名 + TestFlight", () => new IosPackDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§6 Browser（WASM）", "第10部分-阶段27-打包部署更新与CI.md §6", "WASM + PWA", () => new BrowserWasmDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 CI 矩阵", "第10部分-阶段27-打包部署更新与CI.md §8", "GitHub Actions matrix", () => new CiMatrixDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 SBOM 与依赖审计", "第10部分-阶段27-打包部署更新与CI.md §9", "SBOM 生成与审计", () => new SbomAuditDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 崩溃上报与可观测", "第10部分-阶段27-打包部署更新与CI.md §10", "Sentry/崩溃上报", () => new CrashReportDemo()) { Order = 10 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
