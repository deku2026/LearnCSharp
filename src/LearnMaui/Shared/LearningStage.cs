namespace LearnMaui.Shared;

public enum LearningLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert,
}

public sealed record LearningTopic(
    LearningLevel Level,
    string Title,
    string Explanation,
    string Evidence);

public sealed record LearningLab(
    string Title,
    string Goal,
    Func<CancellationToken, Task<string>> RunAsync);

public sealed record LearningStage(
    int Number,
    string Title,
    string Reference,
    IReadOnlyList<LearningTopic> Topics,
    IReadOnlyList<LearningLab> Labs)
{
    public string StageLabel => $"Stage {Number:00}";
}
