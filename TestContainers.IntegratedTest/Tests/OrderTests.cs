namespace TestContainers.IntegratedTest.Tests;

using System;
using System.IO;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using TestContainers.IntegratedTest.Fixtures;
using Xunit;

[Collection("IntegratedTests")]
public sealed class OrderTests : IClassFixture<DemoAppContainer>
{
    private static readonly ChromeOptions _chromeOptions = new()
    {
        AcceptInsecureCertificates = true,

    };

    private readonly DemoAppContainer _demoAppContainer;

    static OrderTests()
    {
        _chromeOptions.AddArgument("headless");
        _chromeOptions.AddArgument("disable-gpu");
        _chromeOptions.AddArgument("no-sandbox");
        _chromeOptions.AddArgument("disable-extensions");
        _chromeOptions.AddArgument("disable-dev-shm-usage");
        _chromeOptions.AddArgument("ignore-certificate-errors");
        _chromeOptions.AddArgument("ignore-ssl-errors=yes");
    }

    public OrderTests(DemoAppContainer demoAppContainer)
    {
        _demoAppContainer = demoAppContainer;
        _demoAppContainer.SetBaseAddress();
    }

    [Fact]
    [Trait("Category", nameof(OrderTests))]
    public void Get_Orders_Should_Return_626_Pages_Of_Orders()
    {
        // Arrange
        static string ScreenshotFileName(string stepName) => $"{nameof(Get_Orders_Should_Return_626_Pages_Of_Orders)}_{stepName}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.png";

        using var chromeService = ChromeDriverService.CreateDefaultService(_chromeOptions);

        chromeService.Port = 42559;

        using ChromeDriver chrome = new(chromeService, _chromeOptions);

        WebDriverWait webDriverWait = new(chrome, TimeSpan.FromSeconds(60));

        // Act
        chrome.Navigate().GoToUrl(_demoAppContainer.BaseAddress);

        chrome.TakeScreenshot().SaveAsFile(Path.Combine(CommonDirectoryPath.GetProjectDirectory().DirectoryPath, ScreenshotFileName("Page_Load")));

        webDriverWait.Until(x => x.FindElement(By.Id("orders_link"))).Click();

        chrome.TakeScreenshot().SaveAsFile(Path.Combine(CommonDirectoryPath.GetProjectDirectory().DirectoryPath, ScreenshotFileName("Order_Overview")));

        var span = webDriverWait.Until(x => x.FindElement(By.ClassName("pager-display")).FindElement(By.TagName("span")));

        // Assert
        _ = span.Text.Should().Be("1 of 626");
    }
}
