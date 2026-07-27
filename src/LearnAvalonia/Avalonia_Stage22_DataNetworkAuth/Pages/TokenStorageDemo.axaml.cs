// Doc:    第9部分-阶段22-数据存储网络与认证.md §11
// Title:  Token 存储与内存模型
// Stage:  Stage22
// Accept: access 内存 + refresh 安全存储
// Notes:  提前 1-2 分钟刷新；锁屏可选清内存 access。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class TokenStorageDemo : UserControl
{
    public TokenStorageDemo()
    {
        InitializeComponent();
    }
}
