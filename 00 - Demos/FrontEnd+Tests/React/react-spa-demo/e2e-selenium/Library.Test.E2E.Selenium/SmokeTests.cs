using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Test.E2E.Selenium;

public class SmokeTests : IDisposable
{
    // our first selenium test.
    // We eed an instance of our driver - matched to our browser

    private readonly ChromeDriver _driver;

    public SmokeTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,900");
        
        _driver = new ChromeDriver(options);

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
    }

    public void Dispose()
    {
        _driver.Quit(); // kills the browser AND the chromedriver process
    }

    [Fact]
    public void OpenTheSpa_ShowsTitleAndHeading()
    {
        // Act - a real mavigation in a real browser
        _driver.Navigate().GoToUrl("http://localhost:5173/");

        // Assert - the document title and the header react renders
        _driver.Title.Should().Be("Library - Catalog");
        _driver.FindElement(By.TagName("h1")).Text.Should().Be("Library");
    }

    [Fact]
    public void Catalog()
    {
        _driver.Navigate().GoToUrl("");
        
        var cards = _driver.FindElements(By.CssSelector("article.card"));
        cards.Should().NotBeEmpty();
    }
}