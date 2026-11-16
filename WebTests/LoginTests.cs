using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WebTests;

[TestFixture]
[Category("LoginTests")]
public class LoginTests
{
    private const bool Headless = true;
    private IWebDriver _driver = null;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        if (Headless)
        {
            options.AddArgument("--headless=new");
        }

        options.AddArgument("--window-size=1366,768");
        _driver = new ChromeDriver(options);
    }

    [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Test]
        public void Login_WithValidCredentials_ShowsProductPage()
        {
            // Arrange: open the login page 
            _driver.Navigate().GoToUrl("https://www.saucedemo.com/");

            // Act: log in as the standard test user
            _driver.FindElement(By.Id("user-name")).SendKeys("standard_user");
            _driver.FindElement(By.Id("password")).SendKeys("secret_sauce");
            _driver.FindElement(By.Id("login-button")).Click();

            // Wait until the Products heading is available
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var title = wait.Until(d =>
            {
                var el = d.FindElement(By.CssSelector("[data-test='title']"));
                return el.Displayed ? el : null;
            });

            // Assert: we reached the inventory page and it shows the Products title
            Assert.That(_driver.Url, Does.EndWith("/inventory.html"));
            Assert.That(title.Text, Is.EqualTo("Products"));
        }
}