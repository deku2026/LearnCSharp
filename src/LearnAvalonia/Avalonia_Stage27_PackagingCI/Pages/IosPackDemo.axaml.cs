// Doc:    第10部分-阶段27-打包部署更新与CI.md §5
// Title:  iOS / iPadOS
// Stage:  Stage27
// Accept: IPA + 签名 + TestFlight
// Notes:  dotnet workload install ios；IPA；签名 Provisioning Profile；TestFlight 分发。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

public partial class IosPackDemo : UserControl
{
    public IosPackDemo()
    {
        InitializeComponent();
    }
}
