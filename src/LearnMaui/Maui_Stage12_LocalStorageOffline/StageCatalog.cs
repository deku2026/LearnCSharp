using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage12_LocalStorageOffline;

internal static class StageCatalog
{
    private static readonly Guid LabNoteId =
        Guid.Parse("00000000-0000-0000-0000-000000000012");

    public static LearningStage Create(IOfflineNoteRepository repository)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "目录选型",
                "AppDataDirectory 保存需备份的应用私有数据，CacheDirectory 可随时被清理；用户选择文件保留授权句柄/复制，不假设平台绝对路径。",
                "database under FileSystem.AppDataDirectory"),
            new(LearningLevel.Beginner, "安全文件写入",
                "写临时文件、Flush、校验后原子替换，异常保留旧文件；删除前解析精确路径并限制在应用目录，流与句柄用 await using。",
                "repository owns one exact database path"),
            new(LearningLevel.Beginner, "Preferences 边界",
                "Preferences 只放主题、最后 tab 等非敏感小键值；列表、草稿和大 JSON 不属于键值设置。",
                "notes live in SQLite, not Preferences"),
            new(LearningLevel.Beginner, "SecureStorage 边界",
                "SecureStorage 放 token/密钥等机密小值，处理密钥失效与卸载重装；它不是查询型业务数据库。",
                "database contains domain data, credentials stay separate"),
            new(LearningLevel.Beginner, "SQLite 基础",
                "先掌握 schema、参数化 SQL、事务、索引、null/default 与连接生命周期，再选择 sqlite-net 或 EF Core。",
                "Microsoft.Data.Sqlite direct implementation"),
            new(LearningLevel.Intermediate, "事务是业务边界",
                "本地实体变化与 outbox 操作同一事务提交，避免 UI 已更新却没有同步意图；失败整体回滚。",
                "SaveAsync/DeleteAsync transaction"),
            new(LearningLevel.Intermediate, "参数化与注入防护",
                "所有用户数据用 $parameter，不拼 SQL；表/列名只能来自代码白名单。",
                "SqliteParameter AddWithValue"),
            new(LearningLevel.Intermediate, "索引按查询形状",
                "索引服务真实 WHERE+ORDER BY，本例按 is_deleted、updated_utc 支持活动笔记倒序；多索引会增加写成本。",
                "ix_notes_updated"),
            new(LearningLevel.Intermediate, "WAL 与并发",
                "WAL 改善读写并发但不取消单写者约束；连接短持有、事务短小，应用层序列化迁移并处理 busy。",
                "PRAGMA journal_mode=WAL"),
            new(LearningLevel.Intermediate, "Repository 边界",
                "VM 依赖 CRUD/查询接口，不知道 SQL、路径和持久实体；Repository 输出 Domain 并接受 CancellationToken。",
                "IOfflineNoteRepository"),
            new(LearningLevel.Intermediate, "离线优先读路径",
                "页面先读本地 SSOT 立即显示，再在允许时同步；网络返回写 DB，观察/重查 DB 驱动 UI，避免网络与内存双真相源。",
                "ListAsync reads local truth"),
            new(LearningLevel.Advanced, "乐观写路径",
                "用户修改立即事务写本地+outbox，UI 从本地读取；后台同步成功清队列，失败保留并显示待同步标记。",
                "SaveAsync + outbox"),
            new(LearningLevel.Advanced, "幂等",
                "实体使用稳定 ID，UPSERT 同 ID 更新而不重复；同步操作带 operation/idempotency id，服务器重复接收不造重复行。",
                "ON CONFLICT(id) DO UPDATE"),
            new(LearningLevel.Advanced, "软删除与 tombstone",
                "离线删除不能立即忘记，需要 tombstone 同步给服务端；同步确认和保留策略到期后再物理清理。",
                "is_deleted + delete outbox operation"),
            new(LearningLevel.Advanced, "迁移",
                "schema 通过 user_version/版本表逐步迁移，每一步可恢复、事务化、真机旧数据升级验证；禁止卸载应用代替迁移。",
                "PRAGMA user_version=1"),
            new(LearningLevel.Advanced, "冲突策略",
                "明确 last-write-wins、服务器权威、字段合并或人工冲突；时间戳只在时钟可信时使用，version/etag 更稳。",
                "Note.Version is persisted for conflict decisions"),
            new(LearningLevel.Advanced, "缓存失效",
                "TTL、etag/version、用户手动刷新和推送失效组合使用；离线时展示陈旧时间，不因过期直接清空可用内容。",
                "UpdatedAt retained with local row"),
            new(LearningLevel.Expert, "加密数据库",
                "SQLCipher 等加密需管理密钥生成、SecureStorage、轮换、备份和性能；普通 SQLite 文件权限不等于静态加密。",
                "encryption intentionally not claimed by this plain SQLite sample"),
            new(LearningLevel.Expert, "ORM 选型",
                "sqlite-net 轻量，EF Core 功能丰富但体积/AOT/迁移成本更高，LiteDB 是可选文档库；以查询、包体和真机数据决策。",
                "current repository uses package already present in solution"),
            new(LearningLevel.Expert, "登出与安全删除",
                "登出按用户分区清 DB/文件、token、缓存和后台任务；先停止使用者再删除精确文件，并说明闪存无法保证物理擦除。",
                "repository exposes exact DatabasePath for controlled ownership"),
        ];

        LearningLab[] labs =
        [
            new("初始化与迁移", "创建数据库、WAL、表、索引和 schema version；重复调用必须幂等。",
                async token =>
                {
                    await repository.InitializeAsync(token);
                    await repository.InitializeAsync(token);
                    return $"Database={repository.DatabasePath}\nExists={File.Exists(repository.DatabasePath)}";
                }),
            new("离线 UPSERT", "以稳定 ID 写入本地实体并原子追加 outbox，然后从本地 SSOT 读取。",
                async token =>
                {
                    Note note = new(
                        LabNoteId,
                        "Offline-first MAUI",
                        $"Saved at {DateTimeOffset.Now:O}",
                        DateTimeOffset.UtcNow,
                        1);
                    await repository.SaveAsync(note, token);
                    IReadOnlyList<Note> notes = await repository.ListAsync(token);
                    int pending = await repository.GetPendingOperationCountAsync(token);
                    return $"visible={notes.Count}, lab note={notes.Count(item => item.Id == LabNoteId)}, pending={pending}";
                }),
            new("验证幂等实体", "连续保存同一实体 ID，确认活动表只有一行而 outbox 保留两次操作意图。",
                async token =>
                {
                    for (int version = 2; version <= 3; version++)
                    {
                        await repository.SaveAsync(
                            new Note(
                                LabNoteId,
                                $"Version {version}",
                                "same id",
                                DateTimeOffset.UtcNow,
                                version),
                            token);
                    }

                    IReadOnlyList<Note> notes = await repository.ListAsync(token);
                    Note? lab = notes.SingleOrDefault(item => item.Id == LabNoteId);
                    return $"entity rows={notes.Count(item => item.Id == LabNoteId)}, version={lab?.Version}";
                }),
            new("模拟同步确认", "读取待同步数量并在模拟服务器成功后清空 outbox。",
                async token =>
                {
                    int before = await repository.GetPendingOperationCountAsync(token);
                    int deleted = await repository.MarkAllSyncedAsync(token);
                    int after = await repository.GetPendingOperationCountAsync(token);
                    return $"before={before}, acknowledged={deleted}, after={after}";
                }),
            new("软删除", "将实体标记 tombstone 并加入删除 outbox；活动查询不再返回它。",
                async token =>
                {
                    await repository.DeleteAsync(LabNoteId, token);
                    IReadOnlyList<Note> notes = await repository.ListAsync(token);
                    int pending = await repository.GetPendingOperationCountAsync(token);
                    return $"visible lab note={notes.Any(item => item.Id == LabNoteId)}, pending={pending}";
                }),
        ];

        return new LearningStage(
            12,
            "本地存储与离线优先",
            "第5部分-阶段12-本地存储与离线优先.md",
            topics,
            labs);
    }
}
