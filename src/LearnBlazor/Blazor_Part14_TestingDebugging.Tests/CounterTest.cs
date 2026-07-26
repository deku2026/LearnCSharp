namespace LearnBlazor.blazor_Part14_Tests;

public sealed class CounterTest : TestContext
{
    [Fact]
    public void Renders_start_value()
    {
        // Act
        IRenderedComponent<Counter> cut = RenderComponent<Counter>(p => p.Add(c => c.Start, 5));

        // Assert
        Assert.Contains("Count: 5", cut.Markup);
    }

    [Fact]
    public void Increments_on_click()
    {
        // Arrange
        IRenderedComponent<Counter> cut = RenderComponent<Counter>(p => p.Add(c => c.Start, 5));

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.Equal("Count: 6", cut.Find("p").TextContent);
    }
}