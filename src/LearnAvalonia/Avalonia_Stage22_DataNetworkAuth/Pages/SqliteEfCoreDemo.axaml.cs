// Doc:    第9部分-阶段22-数据存储网络与认证.md §5
// Title:  SQLite 与 EF Core
// Stage:  Stage22
// Accept: SQLite 可读写
// Notes:  使用 :memory: 连接，不写文件；Microsoft.Data.Sqlite 手写 SQL。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class SqliteEfCoreDemo : UserControl
{
    public SqliteEfCoreDemo()
    {
        InitializeComponent();
    }

    private void CreateAndInsertButton_Click(object? sender, RoutedEventArgs e)
    {
        // :memory: 连接 —— 不写盘，进程退出即消失；演示真实 SqliteConnection。
        using SqliteConnection conn = new("Data Source=:memory:");
        conn.Open();
        using (SqliteCommand create = conn.CreateCommand())
        {
            create.CommandText = "CREATE TABLE notes (id INTEGER PRIMARY KEY, title TEXT NOT NULL, done INTEGER NOT NULL DEFAULT 0)";
            create.ExecuteNonQuery();
        }
        int inserted = 0;
        for (int i = 1; i <= 3; i++)
        {
            using SqliteCommand insert = conn.CreateCommand();
            insert.CommandText = "INSERT INTO notes (title, done) VALUES ($title, $done)";
            insert.Parameters.AddWithValue("$title", $"演示笔记 {i}");
            insert.Parameters.AddWithValue("$done", i % 2 == 0 ? 1 : 0);
            inserted += insert.ExecuteNonQuery();
        }
        if (SqliteLog is not null)
        {
            SqliteLog.Text = $"建表 notes + 插入 {inserted} 行完成。\n下一步点「查询全部」读取。";
        }
    }

    private void QueryButton_Click(object? sender, RoutedEventArgs e)
    {
        // 每次 query 都新开 :memory:（演示用）；生产用持久文件 LocalApplicationData/MyApp/app.db。
        using SqliteConnection conn = new("Data Source=:memory:");
        conn.Open();
        using (SqliteCommand create = conn.CreateCommand())
        {
            create.CommandText = "CREATE TABLE notes (id INTEGER PRIMARY KEY, title TEXT NOT NULL, done INTEGER NOT NULL DEFAULT 0); INSERT INTO notes (title, done) VALUES ('A', 0), ('B', 1), ('C', 0);";
            create.ExecuteNonQuery();
        }
        using SqliteCommand query = conn.CreateCommand();
        query.CommandText = "SELECT id, title, done FROM notes ORDER BY id";
        System.Text.StringBuilder sb = new();
        using SqliteDataReader reader = query.ExecuteReader();
        while (reader.Read())
        {
            sb.AppendLine($"  id={reader.GetInt32(0)} title={reader.GetString(1)} done={reader.GetInt32(2)}");
        }
        if (SqliteLog is not null)
        {
            SqliteLog.Text = $"查询结果（:memory: 内新建 + 插入 + 读取）：\n{sb}";
        }
    }
}
