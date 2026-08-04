using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class NavigationTests: IDisposable
{
    private readonly ChromeDriver _driver;

    public NavigationTests()
    {

        // Option classes: per browser launch config.
        // Headless makes it so chrome doesn't pop up
        // we can even tell it things like what window size we want it to use
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,900");

        // Creating our driver with the options above
        _driver = new ChromeDriver(options);

        // We can also use the constructor to configure an implicit wait
        // We will set it so each FindElement(s) retries for up to 2s before 
        // failing. Proper explicit waits will be demoed later on.
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

        _driver.Navigate().GoToUrl("http://localhost:5173/");

    }

    public void Dispose()
    {
        _driver.Quit(); // kills the browser AND the chromedriver process
    }

    [Fact]
    public void ByTagName_FindsTheHeader()
    {
        _driver.FindElement(By.TagName("h1")).Text.Should().Be("Library");
    }

    [Fact]
    public void ByClassName_FindsEveryCard()
    {
        var cards = _driver.FindElements(By.ClassName("card"));

        cards.Should().NotBeEmpty();
    }

    [Fact]
    public void ByCssSelector_ComposesStructureAndClass()
    {
        var firstTitleLink = _driver.FindElement(By.CssSelector("article.card h3 a"));

        firstTitleLink.Text.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ByLinkText_FindsAnchorsByWhatUserReads()
    {
        _driver.FindElement(By.LinkText("About")).TagName.Should().Be("a");
        _driver.FindElement(By.PartialLinkText("Cata")).Text.Should().Be("Catalog");
    }

    [Fact]
    public void DirectUrl_LoadsADeepRoute()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/inventory/BK-001");
        _driver.FindElement(By.TagName("h2")).Text.Should().Be("Clean Code");
    }

    [Fact]
    public void BackForwardRefresh_WalkTheHistory()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/");
        _driver.Navigate().GoToUrl("http://localhost:5173/about");;

        _driver.FindElement(By.TagName("h2")).Text.Should().Be("About");

        _driver.Navigate().Back();
        _driver.FindElement(By.TagName("h2")).Text.Should().Be("Catalog");

        _driver.Navigate().Forward();
        _driver.FindElement(By.TagName("h2")).Text.Should().Be("About");

        _driver.Navigate().Refresh();
        _driver.FindElement(By.TagName("h2")).Text.Should().Be("About");
        _driver.Url.Should().EndWith("/about");
    }
}
