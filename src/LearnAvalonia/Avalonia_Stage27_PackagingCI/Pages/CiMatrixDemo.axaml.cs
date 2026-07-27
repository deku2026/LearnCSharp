// Doc:    第10部分-阶段27-打包部署更新与CI.md §8
// Title:  CI 矩阵
// Stage:  Stage27
// Accept: GitHub Actions matrix
// Notes:  matrix 覆盖 RID；publish artifact；触发 tag 自动发布。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

public partial class CiMatrixDemo : UserControl
{
    public CiMatrixDemo()
    {
        InitializeComponent();
    }
}
