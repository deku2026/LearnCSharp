// Doc:    第10部分-阶段24-测试体系.md §6
// Title:  数据绑定 Headless 测试
// Stage:  Stage24
// Accept: TwoWay 更新/Converter/验证错误
// Notes:  构造 TextBox + ViewModel，模拟 TwoWay 绑定，断言值同步。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class BindingHeadlessTests
{
    [AvaloniaFact]
    public void TextBox_Text_Binds_Back_To_ViewModel_On_LostFocus()
    {
        CounterVm vm = new();
        Avalonia.Controls.TextBox box = new()
        {
            DataContext = vm,
        };
        box.Bind(Avalonia.Controls.TextBox.TextProperty, new Avalonia.Data.Binding(nameof(CounterVm.Name)) { Mode = Avalonia.Data.BindingMode.TwoWay });
        box.Measure(new Avalonia.Size(200, 30));
        box.Arrange(new Avalonia.Rect(0, 0, 200, 30));

        box.Text = "Hello";
        Assert.Equal("Hello", vm.Name);
    }

    [AvaloniaFact]
    public void OneWay_Binding_Pushes_Value_From_ViewModel_To_Control()
    {
        CounterVm vm = new() { Name = "Pushed" };
        Avalonia.Controls.TextBlock block = new()
        {
            DataContext = vm,
        };
        block.Bind(Avalonia.Controls.TextBlock.TextProperty, new Avalonia.Data.Binding(nameof(CounterVm.Name)));
        block.Measure(new Avalonia.Size(200, 30));
        block.Arrange(new Avalonia.Rect(0, 0, 200, 30));
        Assert.Equal("Pushed", block.Text);
    }

    private sealed class CounterVm : System.ComponentModel.INotifyPropertyChanged
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    }
}
