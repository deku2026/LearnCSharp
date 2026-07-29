using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage10_DevicePermissions;

internal static class StageCatalog
{
    public static LearningStage Create(IDeviceCapabilities capabilities)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "权限双层模型",
                "平台清单声明应用可能使用的能力，运行时再 CheckStatus/Request。缺一不可；只声明不等于用户已授权。",
                "AndroidManifest.xml + Permissions.LocationWhenInUse"),
            new(LearningLevel.Beginner, "最小权限与按需请求",
                "只在用户触发相关功能时说明价值并请求最小权限，不在首屏批量索要。拒绝后仍提供可用降级路径。",
                "GetLocationPermissionAsync(requestIfNeeded)"),
            new(LearningLevel.Beginner, "权限 UX 状态机",
                "Unknown/Denied/Granted/Restricted/Disabled 映射到解释、请求、打开设置或降级 UI；Never ask again 不能靠无限重试。",
                "PermissionStatus + CapabilityOutcome"),
            new(LearningLevel.Beginner, "统一能力结果",
                "平台 API 的 unsupported、disabled、denied、canceled、failed 与 success 语义分开，VM 才能呈现正确状态而不是统一 catch。",
                "CapabilityResult<T>"),
            new(LearningLevel.Beginner, "Connectivity 与 DeviceInfo",
                "NetworkAccess 只表示链路，不保证服务可达；DeviceInfo/Display 用于诊断和适配，不应用来歧视功能或生成稳定用户标识。",
                "CaptureSnapshot"),
            new(LearningLevel.Intermediate, "Geolocation",
                "先检查权限和 IsEnabled，再用带精度、超时和 CancellationToken 的请求；位置是敏感数据，采集与保留都要最小化。",
                "GetLocationAsync"),
            new(LearningLevel.Intermediate, "Geocoding",
                "正/逆地理编码是网络/平台服务，结果可能多条、为空或受区域限制；与定位权限、超时和缓存策略分开。",
                "Geocoding.Default is the MAUI facade"),
            new(LearningLevel.Intermediate, "电池、显示与传感器",
                "Battery/DeviceDisplay 可订阅变化；加速度计、陀螺仪、指南针等先检查支持并在页面消失时停止，避免耗电和事件泄漏。",
                "Battery.Default + DeviceDisplay.Current"),
            new(LearningLevel.Intermediate, "文件与媒体选择",
                "FilePicker/MediaPicker 返回授权文件句柄，及时复制到 AppData/Cache；Android 分区存储下不要假设任意真实路径。",
                "FileSystem.AppDataDirectory"),
            new(LearningLevel.Intermediate, "剪贴板与分享",
                "Clipboard 是用户可见的跨应用数据面，避免复制敏感信息；Share 使用系统选择器并处理取消，不假设目标应用存在。",
                "Clipboard.Default and Share.Default"),
            new(LearningLevel.Intermediate, "Browser、Launcher 与 Maps",
                "打开外部 URI 前校验 scheme，先 CanOpen 再 Launch；Browser 模式与回调场景不同，地图调用需要坐标与平台应用。",
                "Browser.Default / Launcher.Default / Map.Default"),
            new(LearningLevel.Intermediate, "触觉、震动与 TTS",
                "先检查 IsSupported，提供设置开关并尊重无障碍；震动时长、TTS rate/locale 不应硬编码成唯一体验。",
                "HapticFeedback.IsSupported + Vibration.IsSupported"),
            new(LearningLevel.Advanced, "WebAuthenticator",
                "OAuth 使用系统浏览器与回调 URI，state/nonce/PKCE 在服务层校验；MAUI 10 调用带 CancellationToken，Android 回调 Activity 需清单配置。",
                "WebAuthenticator.Default is wrapped behind an auth interface"),
            new(LearningLevel.Advanced, "SecureStorage",
                "令牌等敏感小值放平台安全存储，必须处理设备锁、备份恢复和密钥失效。它不是数据库，也不要记录明文日志。",
                "RoundTripSecretAsync catches platform failures"),
            new(LearningLevel.Advanced, "Preferences",
                "非敏感小配置放 Preferences；复杂对象、草稿和离线数据用文件/SQLite。键集中定义并有迁移/默认值策略。",
                "Preferences is distinct from SecureStorage"),
            new(LearningLevel.Advanced, "平台清单",
                "Android uses-permission/uses-feature、iOS usage descriptions、Windows capabilities、Mac entitlements 均需与实际代码和商店隐私声明一致。",
                "Platforms/Android/AndroidManifest.xml"),
            new(LearningLevel.Advanced, "接口封装与 Fake",
                "VM 依赖 IDeviceCapabilities；生产实现调用 Essentials，Fake 控制权限/定位结果，使 denied/disabled/error 流无需设备即可测试。",
                "MauiDeviceCapabilities + FakeDeviceCapabilities"),
            new(LearningLevel.Expert, "取消与生命周期",
                "权限对话框可能无法被 Token 物理关闭，但请求前后仍检查取消；定位、传感器和认证把 Token 向下传，并在离页停止。",
                "cancellation checks around RequestAsync and GetLocationAsync"),
            new(LearningLevel.Expert, "隐私与商店合规",
                "能力声明、采集目的、保留期限、传输、删除和第三方 SDK 数据必须一致；不为“以后也许用”加入敏感权限。",
                "manifest contains only demonstrated capabilities"),
            new(LearningLevel.Expert, "API 卡片纪律",
                "每项能力记录 facade、平台权限/清单、支持矩阵、线程、取消、异常、隐私、测试替身和降级 UX，避免只记一个调用方法。",
                "CapabilityResult + interface + manifest + labs"),
        ];

        LearningLab[] labs =
        [
            new("读取设备快照", "无权限读取设备、屏幕、电池、网络、触觉与应用目录信息。",
                _ =>
                {
                    DeviceSnapshot snapshot = capabilities.CaptureSnapshot();
                    return Task.FromResult(
                        $"{snapshot.Platform} / {snapshot.Model} / {snapshot.Idiom}\n" +
                        $"Network={snapshot.NetworkAccess} ({snapshot.Profiles})\n" +
                        $"Density={snapshot.Density:F2}, Battery={snapshot.BatteryLevel:P0} {snapshot.BatteryState}\n" +
                        $"Haptics={snapshot.HapticsSupported}, Vibration={snapshot.VibrationSupported}\n" +
                        $"AppData={snapshot.AppDataDirectory}");
                }),
            new("检查定位权限", "先只 CheckStatus，不弹权限框，验证状态可用于预请求说明。",
                async token =>
                {
                    PermissionStatus status =
                        await capabilities.GetLocationPermissionAsync(false, token);
                    return $"LocationWhenInUse={status}";
                }),
            new("请求一次定位", "按需请求权限、检查系统定位开关，再执行有超时和取消的定位。",
                async token =>
                {
                    CapabilityResult<Location> result =
                        await capabilities.GetLocationAsync(token);
                    return result.IsSuccess
                        ? $"lat={result.Value!.Latitude:F5}, lon={result.Value.Longitude:F5}"
                        : $"{result.Outcome}: {result.Message}";
                }),
            new("验证安全存储", "把短期实验值写入 SecureStorage 并读回；平台失败会成为结构化结果。",
                async token =>
                {
                    string secret = $"token-{DateTimeOffset.Now:HHmmss}";
                    CapabilityResult<string> result =
                        await capabilities.RoundTripSecretAsync(secret, token);
                    return $"{result.Outcome}, same={result.Value == secret}, message={result.Message}";
                }),
            new("验证 Fake", "用 Fake 固定拒绝/定位结果，证明 VM 测试不依赖模拟器。",
                async token =>
                {
                    DeviceSnapshot snapshot = capabilities.CaptureSnapshot();
                    FakeDeviceCapabilities fake = new(snapshot)
                    {
                        LocationPermission = PermissionStatus.Denied,
                    };
                    PermissionStatus status =
                        await fake.GetLocationPermissionAsync(false, token);
                    return $"Fake permission={status}; deterministic={status == PermissionStatus.Denied}";
                }),
        ];

        return new LearningStage(
            10,
            "设备能力、权限与平台服务",
            "第4部分-阶段10-设备能力权限平台服务.md",
            topics,
            labs);
    }
}
