using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace JobsWebScraper.Services
{
    public class SeleniumScraper
    {
        private readonly IWebDriver _driver;
        private readonly Actions _actions;
        private readonly IJavaScriptExecutor _ex;
        private readonly Random _rnd;

        public SeleniumScraper()
        {
            // close previous sessions (if any)
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"no previous sessions: {ex.ToString()}");
            }

            var options = new FirefoxOptions();
            options.BinaryLocation = "/home/ian/Projects/firefox/obj-x86_64-pc-linux-gnu/dist/bin/firefox";
            //options.AddArgument("--headless"); // for testing
            // Explicit local driver service so Selenium never invokes its
            // bundled selenium-manager (which phones home to GitHub to
            // resolve/download a matching geckodriver) - avoids a multi-minute
            // hang against the domain-allowlist firewall for no benefit, since
            // a matching geckodriver is already installed locally.
            var service = FirefoxDriverService.CreateDefaultService("/home/ian/.local/bin");
            _driver = new FirefoxDriver(service, options);
            _actions = new Actions(_driver);
            _ex = (IJavaScriptExecutor)_driver;
            _rnd = new Random();
        }

        public async Task<string> GetHtmlAsync(string url)
        {
            await Task.Run(() => _driver.Navigate().GoToUrl(url));
            return _driver.PageSource;
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

        public async Task<ReadOnlyCollection<IWebElement>> WaitForElementsAsync(By by, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
            return await Task.Run(() => wait.Until(d => d.FindElements(by)));
        }
        public async Task<IWebElement> WaitForElementAsync(By by, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
            var el = await Task.Run(() => wait.Until(d => d.FindElement(by)));
            return el;
        }

        public async Task ClickElementAsync(By by, int timeoutSeconds = 10)
        {
            var element = await WaitForElementAsync(by, timeoutSeconds);
            _ex.ExecuteScript("arguments[0].click();", element);
            _actions.MoveToElement(element).Click();
            int randomSleepTime = _rnd.Next(1, 3) * 1000; // from 1 to 3 seconds
            Thread.Sleep(randomSleepTime);
        }

        public async Task ClickElementAsync(IWebElement element, int timeoutSeconds = 10)
        {
            _ex.ExecuteScript("arguments[0].click();", element);
            _actions.MoveToElement(element).Click();
            int randomSleepTime = _rnd.Next(1, 3) * 1000; // from 1 to 3 seconds
            Thread.Sleep(randomSleepTime);
        }

        public async Task SendKeysAsync(By by, string text)
        {
            var element = await WaitForElementAsync(by);
            await Task.Run(() => element.SendKeys(text));
        }

        public async Task<bool> isPresentInDom(By by, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
                await Task.Run(() => wait.Until(d => d.FindElement(by)));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task removeElementFromDOM(By by, int timeoutSeconds = 10)
        {
            var element = await WaitForElementAsync(by, timeoutSeconds);
            _ex.ExecuteScript("arguments[0].remove();", element);
        }

    }
}
