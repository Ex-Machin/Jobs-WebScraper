using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Web;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class SeleniumScraper : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly Actions _actions;
        private readonly IJavaScriptExecutor _ex;

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

            var options = new ChromeOptions();
            options.AddArgument("--headless");
            _driver = new ChromeDriver(options);
            _actions = new Actions(_driver);
            _ex = (IJavaScriptExecutor)_driver;
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
            _actions.MoveToElement(element).Click().Perform();
            Thread.Sleep(5000);

        }

        public async Task SendKeysAsync(By by, string text)
        {
            var element = await WaitForElementAsync(by);
            await Task.Run(() => element.SendKeys(text));
        }
    }

    public class AutomationService : IAutomationService
    {
        private readonly IJobsRepository _repository;
        private static readonly SeleniumScraper scraper = new SeleniumScraper();

        public AutomationService(IJobsRepository repository)
        {
            _repository = repository;
        }

        private async Task scrapeJobs(string company)
        {

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']"));
            Console.WriteLine(jobsList);

            List<Job> jobsLink = new List<Job>();

            foreach (var job in jobsList)
            {
                var columns = job.FindElements(By.TagName("td"));
                string title = columns[0].GetAttribute("textContent");

                var department = columns[1].GetAttribute("textContent");
                var region = columns[2].GetAttribute("textContent");
                var city = columns[3].GetAttribute("textContent");

                Job newJob = new Job();

                newJob.Title = title.ToString();
                newJob.Department = department.ToString();
                newJob.Region = region.ToString();
                newJob.City = city.ToString();
                newJob.Company = company;
                newJob.Status = 0;
                newJob.InterviewRound = 0;

                jobsLink.Add(newJob);
            }

            await _repository.AddJobs(jobsLink);

        }


        public async Task<int> RunAutomation()
        {
            string company = "issworld";
            string fullUrl = "https://www.pl.issworld.com/kariera/oferty-pracy#skk-container";

            await scraper.GetHtmlAsync(fullUrl);

            int recordsUpdatedTotal = 0;

            var lastPagerEl = await scraper.WaitForElementAsync(By.XPath(".//a[@class='skk_pager_last']"));
            int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 1;

            // delete eveything to make sure everything is up to date
            await _repository.DeleteJobByCompany(company);

            await scrapeJobs(company);

            foreach (int _ in Enumerable.Range(1, lastPagerCount))
            {
                await scraper.ClickElementAsync(By.XPath(".//a[@class='skk_pager_next']"));
                await scrapeJobs(company);
            }

            return recordsUpdatedTotal;
        }
    }
}
