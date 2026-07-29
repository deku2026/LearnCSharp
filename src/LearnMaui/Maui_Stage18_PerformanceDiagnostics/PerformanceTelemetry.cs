using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace LearnMaui.Maui_Stage18_PerformanceDiagnostics;

public sealed record PerformanceSample(
    string Name,
    TimeSpan Duration,
    long AllocatedBytes,
    int Gen0Collections,
    int Gen1Collections,
    int Gen2Collections);

public sealed class PerformanceTelemetry : IDisposable
{
    public const string SourceName = "LearnMaui.Stage18";
    private readonly ActivitySource _activitySource = new(SourceName);
    private readonly Meter _meter = new(SourceName, "1.0.0");
    private readonly Histogram<double> _duration;
    private readonly Counter<long> _operations;
    private readonly List<PerformanceSample> _samples = [];
    private readonly object _gate = new();

    public PerformanceTelemetry()
    {
        _duration = _meter.CreateHistogram<double>(
            "learnmaui.operation.duration",
            "ms");
        _operations = _meter.CreateCounter<long>("learnmaui.operation.count");
    }

    public IReadOnlyList<PerformanceSample> Snapshot
    {
        get
        {
            lock (_gate)
            {
                return [.. _samples];
            }
        }
    }

    public OperationScope StartOperation(string name)
    {
        return new OperationScope(this, name);
    }

    public async Task<PerformanceSample> MeasureAsync(
        string name,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken)
    {
        using OperationScope scope = StartOperation(name);
        await operation(cancellationToken);
        return scope.Complete();
    }

    private void Record(PerformanceSample sample, Activity? activity)
    {
        activity?.SetTag("duration.ms", sample.Duration.TotalMilliseconds);
        activity?.SetTag("allocated.bytes", sample.AllocatedBytes);
        _duration.Record(
            sample.Duration.TotalMilliseconds,
            new KeyValuePair<string, object?>("operation", sample.Name));
        _operations.Add(
            1,
            new KeyValuePair<string, object?>("operation", sample.Name));
        lock (_gate)
        {
            _samples.Add(sample);
        }
    }

    public void Dispose()
    {
        _activitySource.Dispose();
        _meter.Dispose();
    }

    public sealed class OperationScope : IDisposable
    {
        private readonly PerformanceTelemetry _owner;
        private readonly string _name;
        private readonly Activity? _activity;
        private readonly long _started = Stopwatch.GetTimestamp();
        private readonly long _allocated = GC.GetTotalAllocatedBytes();
        private readonly int[] _collections =
            [GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2)];
        private PerformanceSample? _sample;

        internal OperationScope(PerformanceTelemetry owner, string name)
        {
            _owner = owner;
            _name = name;
            _activity = owner._activitySource.StartActivity(
                name,
                ActivityKind.Internal);
        }

        public PerformanceSample Complete()
        {
            if (_sample is not null)
            {
                return _sample;
            }

            _sample = new PerformanceSample(
                _name,
                Stopwatch.GetElapsedTime(_started),
                Math.Max(0, GC.GetTotalAllocatedBytes() - _allocated),
                GC.CollectionCount(0) - _collections[0],
                GC.CollectionCount(1) - _collections[1],
                GC.CollectionCount(2) - _collections[2]);
            _owner.Record(_sample, _activity);
            _activity?.Stop();
            return _sample;
        }

        public void Dispose()
        {
            Complete();
            _activity?.Dispose();
        }
    }
}

public static class PerformanceExperiments
{
    public static async Task<(int Notifications, TimeSpan Duration)> AddIndividuallyAsync(
        int count,
        CancellationToken cancellationToken)
    {
        ObservableCollection<int> items = [];
        int notifications = 0;
        items.CollectionChanged += OnCollectionChanged;
        long started = Stopwatch.GetTimestamp();
        for (int index = 0; index < count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            items.Add(index);
        }

        await Task.Yield();
        items.CollectionChanged -= OnCollectionChanged;
        return (notifications, Stopwatch.GetElapsedTime(started));

        void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            notifications++;
        }
    }

    public static async Task<bool> VerifyCollectibleAsync(CancellationToken cancellationToken)
    {
        WeakReference reference = CreateUnrootedReference();
        for (int attempt = 0; attempt < 3 && reference.IsAlive; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            await Task.Yield();
        }

        return !reference.IsAlive;
    }

    public static long EstimateDecodedImageBytes(
        int pixelWidth,
        int pixelHeight,
        int bytesPerPixel = 4)
    {
        return checked((long)pixelWidth * pixelHeight * bytesPerPixel);
    }

    private static WeakReference CreateUnrootedReference()
    {
        object candidate = new();
        return new WeakReference(candidate);
    }
}
