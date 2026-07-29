using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage20_PackagingCI;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "应用身份",
                "ApplicationId/Bundle ID/Package Identity 是商店和升级链身份，发布后不能随意改；各环境用显式后缀与独立后端/签名。",
                "ApplicationId in csproj"),
            new(LearningLevel.Beginner, "版本双轨",
                "DisplayVersion 面向用户语义，ApplicationVersion/versionCode/build number 单调递增供商店比较；CI 从 tag/run 注入并校验。",
                "ReleaseVersion"),
            new(LearningLevel.Beginner, "工具链固定",
                "global.json 固定 .NET SDK，workload/VS/Xcode/Android SDK 版本写入构建记录；CI 与开发机使用同一锁定策略。",
                "repository global.json"),
            new(LearningLevel.Beginner, "配置分层",
                "Debug/Release 与 Dev/Staging/Prod 是不同维度；endpoint/feature 配置可注入，签名和 token 永不提交源码。",
                "Release properties + environment-only signing"),
            new(LearningLevel.Intermediate, "APK 与 AAB",
                "APK 用于本地/企业安装，Play 发布主线用 AAB 由商店生成设备 APK；两者都要安装真实产物做 smoke。",
                "Release AndroidPackageFormats=aab;apk"),
            new(LearningLevel.Intermediate, "Android 签名",
                "keystore、alias 与密码来自 CI Secret/临时文件，日志屏蔽值；Play App Signing 和 upload key 备份/轮换责任清晰。",
                "SigningReadiness checks names, never prints values"),
            new(LearningLevel.Intermediate, "SDK、R8 与 16KB 页",
                "target/min SDK 跟商店门槛，R8/trim 后做行为测试；native 库检查 ABI 和 16KB page compatibility，保留 mapping/symbols。",
                "release artifact verification checklist"),
            new(LearningLevel.Intermediate, "Play 轨道",
                "Internal→Closed/Open→Production 分轨，先小比例 staged rollout；Play Integrity 是风险信号，不替代服务端鉴权。",
                "RolloutPlan"),
            new(LearningLevel.Intermediate, "iOS 身份",
                "Bundle ID、证书、App Store Connect API key、provisioning profile、entitlements 必须匹配；构建在受控 Mac runner。",
                "platform-specific CI job, not Android project code"),
            new(LearningLevel.Intermediate, "TestFlight 与 Privacy",
                "上传 archive+dSYM，内部/外部测试组有审查差异；PrivacyInfo.xcprivacy 与第三方 SDK required-reason API 必须审计。",
                "store metadata/privacy gate"),
            new(LearningLevel.Advanced, "Windows MSIX",
                "MSIX identity、publisher、证书和 Store 关联一致；选择 packaged/unpackaged 与 Windows App SDK runtime/更新策略。",
                "separate Windows runner/artifact"),
            new(LearningLevel.Advanced, "Mac Catalyst 公证",
                "Developer ID/App Store 路径、entitlements、签名顺序、notarization/staple 和符号存档在 Mac job 完成。",
                "platform job matrix"),
            new(LearningLevel.Advanced, "CI runner 矩阵",
                "Android/Windows 用受控 Windows runner，Apple 用 macOS；restore/build/sign/upload 分阶段，缓存键含 SDK/workload/lock。",
                "Android intentionally excluded from current repository CI solution"),
            new(LearningLevel.Advanced, "Secret 纪律",
                "Secret 最小权限、环境隔离、短期 token、审批和轮换；临时 keystore/profile 用后清理，命令不回显。",
                "SigningReadiness only returns missing variable names"),
            new(LearningLevel.Advanced, "产物可追溯",
                "每个产物记录文件名、版本、commit、SHA-256、大小、runner/toolchain 和 symbols；上传/推广同一不可变 artifact。",
                "ArtifactManifest"),
            new(LearningLevel.Advanced, "商店元数据",
                "标题、说明、截图、分级、隐私政策、数据安全、无障碍和联系方式版本化；截图覆盖设备/语言且不含测试数据。",
                "metadata is a release gate, not manual afterthought"),
            new(LearningLevel.Expert, "Feature Flag 与回滚",
                "远程 flag 有默认/缓存/签名/过期/kill switch；回滚服务端与停 rollout 优先，移动二进制回退受商店版本规则限制。",
                "RolloutPlan halt policies"),
            new(LearningLevel.Expert, "热更新边界",
                "不下载执行任意新代码绕商店审核；配置/内容/flag 与二进制功能边界清楚，安全更新仍走签名商店链。",
                "immutable signed artifacts"),
            new(LearningLevel.Expert, "崩溃与混淆",
                "release 上传 dSYM/mapping/native symbols，事件带版本/commit；可选混淆必须验证反射/序列化并保留符号映射。",
                "DebugSymbols and portable PDB configuration"),
            new(LearningLevel.Expert, "完整发布门禁",
                "零未解释警告、依赖审计、签名验证、安装启动、登录/离线/深链/权限 smoke、隐私/元数据、产物 hash 与回滚手册齐全。",
                "code labs model deterministic release evidence"),
        ];

        LearningLab[] labs =
        [
            new("读取应用版本", "读取打包进 Android manifest 的名称、包名、显示版本和 build version。",
                _ => Task.FromResult(
                    $"name={AppInfo.Current.Name}\n" +
                    $"package={AppInfo.Current.PackageName}\n" +
                    $"display={AppInfo.Current.VersionString}\n" +
                    $"build={AppInfo.Current.BuildString}")),
            new("校验版本注入", "解析合法 display/build，并拒绝不完整或非递增 build。",
                unusedToken =>
                {
                    bool valid = ReleaseVersion.TryParse("2.4.1", "20401", out ReleaseVersion version);
                    bool invalid = ReleaseVersion.TryParse("2.4", "0", out _);
                    return Task.FromResult(
                        $"valid={valid}, display={version.Display}, build={version.Build}\ninvalid accepted={invalid}");
                }),
            new("检查签名 Secret", "只检查必需环境变量是否存在，绝不读取到输出或记录 secret 值。",
                _ =>
                {
                    SigningReadiness readiness = SigningReadiness.Inspect();
                    return Task.FromResult(
                        $"ready={readiness.IsReady}\nmissing={string.Join(", ", readiness.MissingVariables)}\nvalues printed=false");
                }),
            new("生成产物清单", "创建临时 artifact，计算 SHA-256/大小/commit 并删除临时文件。",
                async token =>
                {
                    string path = Path.Combine(
                        FileSystem.CacheDirectory,
                        $"stage20-{Guid.NewGuid():N}.artifact");
                    try
                    {
                        await File.WriteAllTextAsync(path, "signed artifact payload", token);
                        ArtifactManifest manifest =
                            await ArtifactInspector.CreateManifestAsync(path, "local-lab", token);
                        return $"{manifest.FileName}\nbytes={manifest.Bytes}\nsha256={manifest.Sha256}\ncommit={manifest.Commit}";
                    }
                    finally
                    {
                        File.Delete(path);
                    }
                }),
            new("验证分阶段发布", "创建 5% Production rollout，并检查崩溃/ANR 自动暂停策略。",
                _ =>
                {
                    RolloutPlan plan = new(
                        ReleaseChannel.Production,
                        5,
                        true,
                        true);
                    return Task.FromResult(
                        $"channel={plan.Channel}, percent={plan.Percentage}, valid={plan.IsValid}, halt crash={plan.HaltOnCrashRegression}");
                }),
            new("生成稳定产物名", "将 app、语义版本、build、RID 和格式编码进不含 secret 的文件名。",
                _ =>
                {
                    ReleaseVersion version = new(2, 4, 1, 20401);
                    return Task.FromResult(ArtifactNaming.Create(
                        "LearnMaui",
                        version,
                        "android-arm64",
                        "aab"));
                }),
        ];

        return new LearningStage(
            20,
            "打包、签名、商店与 CI/CD",
            "第7部分-阶段20-打包签名商店CI.md",
            topics,
            labs);
    }
}
