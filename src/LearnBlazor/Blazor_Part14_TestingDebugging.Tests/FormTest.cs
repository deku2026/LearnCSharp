namespace LearnBlazor.blazor_Part14_Tests;

public sealed class FormTest : TestContext
{
    [Fact]
    public void Shows_success_after_submit()
    {
        // Arrange
        IRenderedComponent<SimpleForm> cut = RenderComponent<SimpleForm>();

        // Act：输入并提交
        cut.Find("input").Change("Alice");
        cut.Find("button").Click();

        // Assert
        Assert.Contains("Hi, Alice", cut.Markup);
    }
}
