namespace LearnCSharp.Topics;

/// <summary>
/// Marks a static <c>int Run(string[] args)</c> method as a learnable topic.
/// <see cref="TopicRegistry"/> reflects every method bearing this attribute at
/// startup and indexes it by <see cref="Id"/>. The id format is
/// <c>stageNN/sectionMM/item_slug</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class LearnTopicAttribute : Attribute
{
    public LearnTopicAttribute(string id)
    {
        Id = id;
    }

    public string Id { get; }
}
