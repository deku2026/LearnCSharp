using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage16_GraphicsMedia;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "GraphicsView 与 IDrawable",
                "GraphicsView 管平台绘制表面和失效，IDrawable.Draw 接收 ICanvas 与 dirtyRect；模型与绘制器可脱离页面测试。",
                "BarChartDrawable : IDrawable"),
            new(LearningLevel.Beginner, "Canvas 状态",
                "颜色、描边、变换和裁剪属于 Canvas 状态；SaveState/RestoreState 用 try/finally 成对，避免一个组件污染后续绘制。",
                "Draw uses SaveState/RestoreState"),
            new(LearningLevel.Beginner, "路径、填充与描边",
                "Fill 绘面积，Stroke 绘边界；复杂形状用 PathF 和填充规则，像素对齐与抗锯齿按输出场景实测。",
                "FillRoundedRectangle + DrawLine"),
            new(LearningLevel.Beginner, "坐标与 DPI",
                "MAUI Graphics 使用设备无关单位，dirtyRect 是当前可绘区域；不要把物理像素、图片像素和布局单位混成一套。",
                "layout derives from dirtyRect"),
            new(LearningLevel.Beginner, "Invalidate",
                "模型变化后调用 GraphicsView.Invalidate 请求下一帧，Draw 只读取快照；不能在 Draw 内改绑定导致无限重绘。",
                "GraphicsView.Drawable + external model update"),
            new(LearningLevel.Intermediate, "触控绘图",
                "StartInteraction/DragInteraction/EndInteraction 管一条 stroke 生命周期；坐标采样去噪/限距，绘制与输入数据结构分离。",
                "StrokeBuffer"),
            new(LearningLevel.Intermediate, "指针与触控统一",
                "触摸、鼠标和笔最终转成同一 PointF/命令流，同时保留 hover、压力等平台增强；拖动期间处理取消/越界。",
                "platform input feeds StrokeBuffer"),
            new(LearningLevel.Intermediate, "命中测试",
                "绘制阶段缓存几何区域，交互阶段把点与 Rect/Path 比较，返回稳定数据索引而非持有平台 View。",
                "BarChartDrawable.HitTest"),
            new(LearningLevel.Intermediate, "绘制性能",
                "Draw 热路径避免 LINQ/大分配、图片解码和 I/O；只重绘变化区域，复杂缓存要有尺寸/主题失效策略。",
                "bar rectangles reused in one list"),
            new(LearningLevel.Intermediate, "文本与图像",
                "文本测量、字体 fallback 和 RTL 需文化实测；IImage 有明确 Dispose 所有权，图片按目标尺寸解码。",
                "media is copied/decoded outside Draw"),
            new(LearningLevel.Advanced, "何时用 SkiaSharp",
                "需要复杂滤镜、图片编解码、GPU/跨平台一致渲染或既有 Skia 生态时再引入；简单图表/签名 MAUI Graphics 足够。",
                "no Skia package added for this bounded lab"),
            new(LearningLevel.Advanced, "可复用绘制控件",
                "控件暴露 BindableProperty/命令和可访问摘要，内部持有 drawable 与输入状态；尺寸、主题变化统一触发 Invalidate。",
                "GraphicsViewFactory creates semantic chart surface"),
            new(LearningLevel.Advanced, "MediaElement 生命周期",
                "生产媒体用 CommunityToolkit MediaElement 版本矩阵并注册；Appearing 播放、Disappearing 暂停/释放，音频焦点和后台策略按平台。",
                "package absent in this project, so no fake player implementation"),
            new(LearningLevel.Advanced, "MediaPicker",
                "相机/相册结果是 FileResult stream；多选、压缩和平台选择器能力按版本检查，取消是正常路径。",
                "MediaImportPipeline accepts FileResult"),
            new(LearningLevel.Advanced, "EXIF 与隐私",
                "照片需处理方向、GPS EXIF 和上传前元数据策略；不要把原始路径/EXIF 写日志，用户应知道上传内容。",
                "pipeline creates an app-owned cache copy"),
            new(LearningLevel.Advanced, "内存与临时文件",
                "流式复制并限制大小，不把视频/原图整体读 byte[]；缓存有清理策略，最终资产原子移到 AppData。",
                "80 KiB buffer + 25 MiB guard"),
            new(LearningLevel.Expert, "图像处理管线",
                "读取元数据→按目标尺寸解码→方向校正→变换/裁剪→编码→原子写；每阶段可取消、限内存并保留质量策略。",
                "stream boundary is ready for a codec implementation"),
            new(LearningLevel.Expert, "GPU 边界",
                "高帧率/大量粒子/滤镜先 profile CPU、GPU、内存和上传成本；平台 GPU 资源必须跟 surface 生命周期释放。",
                "simple retained data avoids claiming GPU acceleration"),
            new(LearningLevel.Expert, "无障碍替代",
                "自绘图表对读屏不可见，必须提供摘要、可聚焦数据列表和键盘导航；颜色/形状不是唯一编码。",
                "SemanticProperties description + underlying Values"),
            new(LearningLevel.Expert, "测试",
                "纯测布局几何、stroke 限距、hit test 和媒体大小限制；截图/golden 辅助视觉回归，真机测笔、触控、编解码和生命周期。",
                "pure drawable/model seams"),
        ];

        LearningLab[] labs =
        [
            new("构造图表 Surface", "创建带 IDrawable、AutomationId 和读屏摘要的 GraphicsView。",
                _ =>
                {
                    BarChartDrawable drawable = new();
                    GraphicsView view = GraphicsViewFactory.CreateChart(drawable);
                    return Task.FromResult(
                        $"drawable={view.Drawable?.GetType().Name}, height={view.HeightRequest}, automation={view.AutomationId}");
                }),
            new("执行离屏绘制", "用 PictureCanvas 执行真实 Draw，并在缓存几何上进行命中测试。",
                _ =>
                {
                    BarChartDrawable drawable = new();
                    PictureCanvas canvas = new(0, 0, 320, 240);
                    drawable.Draw(canvas, new RectF(0, 0, 320, 240));
                    int hit = drawable.HitTest(new PointF(40, 190));
                    return Task.FromResult($"picture size=320x240, hit index={hit}");
                }),
            new("验证 stroke 限距", "过滤过密采样点，保留足够移动距离的点。",
                _ =>
                {
                    StrokeBuffer buffer = new(3);
                    buffer.Start(new PointF(0, 0));
                    bool near = buffer.Add(new PointF(1, 1));
                    bool far = buffer.Add(new PointF(5, 5));
                    buffer.End();
                    return Task.FromResult(
                        $"near accepted={near}, far accepted={far}, points={buffer.Strokes.Single().Count}");
                }),
            new("验证 hit test", "先绘制缓存 bar Rect，再对柱内与柱外坐标测试。",
                _ =>
                {
                    BarChartDrawable drawable = new();
                    PictureCanvas canvas = new(0, 0, 400, 240);
                    drawable.Draw(canvas, new RectF(0, 0, 400, 240));
                    int inside = drawable.HitTest(new PointF(40, 190));
                    int outside = drawable.HitTest(new PointF(2, 2));
                    return Task.FromResult($"inside={inside}, outside={outside}");
                }),
            new("查看媒体能力", "检查相机/相册 API 支持；真正选择由用户触发并进入流式导入管线。",
                _ => Task.FromResult(
                    $"CapturePhotoSupported={MediaPicker.Default.IsCaptureSupported}\n" +
                    $"Cache={FileSystem.CacheDirectory}\nMax import=25 MiB")),
        ];

        return new LearningStage(
            16,
            "Graphics、媒体与高级交互",
            "第6部分-阶段16-Graphics媒体高级交互.md",
            topics,
            labs);
    }
}
