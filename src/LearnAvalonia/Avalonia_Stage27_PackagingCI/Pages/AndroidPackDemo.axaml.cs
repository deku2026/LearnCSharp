// Doc:    第10部分-阶段27-打包部署更新与CI.md §4
// Title:  Android
// Stage:  Stage27
// Accept: AAB/APK + 签名
// Notes:  dotnet workload install android；AAB 上 Play；APK 自分发；签名 keystore。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

public partial class AndroidPackDemo : UserControl
{
    public AndroidPackDemo()
    {
        InitializeComponent();
    }
}
