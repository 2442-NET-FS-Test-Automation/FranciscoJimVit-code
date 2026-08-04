using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class XPathTests: IDisposable
{
    private readonly ChromeDriver _driver;

    public XPathTests()
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
    public void RelativeXPath_MatchesByAttribute()
    {
        var cards = _driver.FindElements(By.XPath("//article[@class='card']"));
        cards.Should().NotBeEmpty();
    }

    [Fact]
    public void XPathFunctions_MatchOnTExt()
    {
        var cleanCode = _driver.FindElement(By.XPath("//h3/a[contains(text(), 'Clean')]"));
        cleanCode.Text.Should().Be("Clean Code");

        var skus = _driver.FindElements(By.XPath("//dd[starts-with(text(), 'BK-')]"));
        skus.Should().HaveCount(3);
    }

    [Fact]
    public void XPathAxes_WalkUpAndSideways()
    {
        var cardOfCleanCode = _driver.FindElement(
            By.XPath("//a[text()='Clean Code']/ancestor::article")
        );

        cardOfCleanCode.GetAttribute("class").Should().Be("card");

        var firstSku = _driver.FindElement(
            By.XPath("//dt[text()='SKU']/following-sibling::dd[1]")
        );

        firstSku.Text.Should().Be("BK-001");
    }

    [Fact]
    public void AbsoluteXPath_WorksToday()
    {
        // may not works tomorrow
        var h1 = _driver.FindElement(
            By.XPath("/html/body/div/div/header/h1")
        );

        h1.Text.Should().Be("Library");
    }
}