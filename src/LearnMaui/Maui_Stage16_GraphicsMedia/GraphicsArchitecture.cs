namespace LearnMaui.Maui_Stage16_GraphicsMedia;

public sealed class BarChartDrawable : IDrawable
{
    private readonly List<RectF> _bars = [];

    public IReadOnlyList<float> Values { get; set; } = [20, 55, 35, 80];

    public Color Fill { get; set; } = Colors.CornflowerBlue;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        try
        {
            canvas.Antialias = true;
            canvas.FillColor = Colors.Transparent;
            canvas.FillRectangle(dirtyRect);
            canvas.StrokeColor = Colors.SlateGray;
            canvas.StrokeSize = 1;
            canvas.DrawLine(
                dirtyRect.Left + 24,
                dirtyRect.Bottom - 24,
                dirtyRect.Right - 12,
                dirtyRect.Bottom - 24);

            _bars.Clear();
            if (Values.Count == 0)
            {
                return;
            }

            float max = Math.Max(1, Values.Max());
            float availableWidth = dirtyRect.Width - 48;
            float gap = 8;
            float barWidth = Math.Max(2, (availableWidth - gap * (Values.Count - 1)) / Values.Count);
            float availableHeight = dirtyRect.Height - 48;
            canvas.FillColor = Fill;
            for (int index = 0; index < Values.Count; index++)
            {
                float height = availableHeight * Values[index] / max;
                RectF bar = new(
                    dirtyRect.Left + 28 + index * (barWidth + gap),
                    dirtyRect.Bottom - 24 - height,
                    barWidth,
                    height);
                _bars.Add(bar);
                canvas.FillRoundedRectangle(bar, 4);
            }
        }
        finally
        {
            canvas.RestoreState();
        }
    }

    public int HitTest(PointF point)
    {
        return _bars.FindIndex(rectangle => rectangle.Contains(point));
    }
}

public sealed class SignatureDrawable : IDrawable
{
    public IReadOnlyList<IReadOnlyList<PointF>> Strokes { get; set; } = [];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.DarkSlateBlue;
        canvas.StrokeSize = 3;
        canvas.StrokeLineCap = LineCap.Round;
        foreach (IReadOnlyList<PointF> stroke in Strokes)
        {
            for (int index = 1; index < stroke.Count; index++)
            {
                canvas.DrawLine(stroke[index - 1], stroke[index]);
            }
        }
    }
}

public sealed class StrokeBuffer(float minimumDistance = 2)
{
    private readonly List<List<PointF>> _strokes = [];
    private List<PointF>? _current;

    public IReadOnlyList<IReadOnlyList<PointF>> Strokes =>
        _strokes.Select(stroke => (IReadOnlyList<PointF>)stroke).ToArray();

    public void Start(PointF point)
    {
        _current = [point];
        _strokes.Add(_current);
    }

    public bool Add(PointF point)
    {
        if (_current is null)
        {
            return false;
        }

        PointF previous = _current[^1];
        float deltaX = point.X - previous.X;
        float deltaY = point.Y - previous.Y;
        if ((deltaX * deltaX) + (deltaY * deltaY) <
            minimumDistance * minimumDistance)
        {
            return false;
        }

        _current.Add(point);
        return true;
    }

    public void End()
    {
        _current = null;
    }
}

public static class GraphicsViewFactory
{
    public static GraphicsView CreateChart(BarChartDrawable drawable)
    {
        GraphicsView view = new()
        {
            Drawable = drawable,
            HeightRequest = 240,
            AutomationId = "Stage16BarChart",
        };
        SemanticProperties.SetDescription(
            view,
            $"Bar chart with {drawable.Values.Count} values");
        return view;
    }
}

public sealed record CachedMedia(
    string Path,
    string FileName,
    long Bytes,
    string ContentType);

public static class MediaImportPipeline
{
    private const long MaximumBytes = 25 * 1024 * 1024;

    public static async Task<CachedMedia> CopyToCacheAsync(
        FileResult result,
        CancellationToken cancellationToken)
    {
        string safeName = Path.GetFileName(result.FileName);
        string destination = Path.Combine(
            FileSystem.CacheDirectory,
            $"{Guid.NewGuid():N}-{safeName}");
        await using Stream source = await result.OpenReadAsync();
        await using FileStream target = new(
            destination,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous);
        byte[] buffer = new byte[81920];
        long total = 0;
        int read;
        while ((read = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            total += read;
            if (total > MaximumBytes)
            {
                throw new InvalidDataException("Selected media exceeds 25 MiB.");
            }

            await target.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }

        return new CachedMedia(
            destination,
            safeName,
            total,
            result.ContentType ?? "application/octet-stream");
    }
}
