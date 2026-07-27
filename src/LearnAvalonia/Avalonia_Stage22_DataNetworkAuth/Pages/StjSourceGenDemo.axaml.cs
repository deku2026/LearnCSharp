// Doc:    第9部分-阶段22-数据存储网络与认证.md §2
// Title:  STJ 与 Source Gen
// Stage:  Stage22
// Accept: STJ source gen 主 DTO
// Notes:  反射序列化在 trim/AOT 下脆弱；source gen 生成明确契约。
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class StjSourceGenDemo : UserControl
{
    // STJ source gen 上下文：AOT/trim 友好，反射零依赖。
    [JsonSerializable(typeof(NoteDto))]
    [JsonSerializable(typeof(System.Collections.Generic.List<NoteDto>))]
    internal partial class AppJsonContext : JsonSerializerContext
    {
    }

    public sealed record NoteDto(int Id, string Title, bool Done);

    public StjSourceGenDemo()
    {
        InitializeComponent();
    }

    private void RoundTripButton_Click(object? sender, RoutedEventArgs e)
    {
        System.Collections.Generic.List<NoteDto> notes = new()
        {
            new(1, "买菜", false),
            new(2, "写 Avalonia demo", true),
            new(3, "看 OAuth2 文档", false),
        };
        string json = JsonSerializer.Serialize(notes, AppJsonContext.Default.ListNoteDto);
        System.Collections.Generic.List<NoteDto>? back = JsonSerializer.Deserialize(json, AppJsonContext.Default.ListNoteDto);
        if (StjLog is not null)
        {
            StjLog.Text = $"source gen 序列化往返：\nJSON（{json.Length} 字节）：\n{json}\n\n反序列化得 {back?.Count ?? 0} 条：\n  {string.Join("\n  ", back ?? new System.Collections.Generic.List<NoteDto>())}";
        }
    }
}
