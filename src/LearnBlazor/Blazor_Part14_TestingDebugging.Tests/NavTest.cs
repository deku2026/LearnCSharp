namespace LearnBlazor.blazor_Part14_Tests;

public sealed class NavTest : TestContext
{
    [Fact]
    public void Counter_renders_and_increments_without_webview()
    {
        // Arrange
        IRenderedComponent<Counter> cut = RenderComponent<Counter>(p => p.Add(c => c.Start, 0));

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.Contains("Count: 1", cut.Markup);
    }
}
