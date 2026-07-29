namespace LearnMaui.Maui_Stage21_BlazorHybrid;

public sealed class HybridBridge
{
    private const int MaximumMessageLength = 1024;

    public string Echo(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        if (message.Length > MaximumMessageLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(message),
                $"Bridge messages are limited to {MaximumMessageLength} characters.");
        }

        return $"echo:{message}";
    }

    public async Task<string> GetDeviceNameAsync()
    {
        await Task.Yield();
        return DeviceInfo.Current.Name;
    }
}
