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

        public async Task ClickElementAsync(IWebElement element, int timeoutSeconds = 10)
        {
            _ex.ExecuteScript("arguments[0].click();", element);
            _actions.MoveToElement(element).Click();
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

        private async Task scrapeJobsISS(string company)
        {

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']"));

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

        private async Task scrapeJobsMacgregor(string company)
        {
            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='data-row']"));

            List<Job> jobsLink = new List<Job>();

            foreach (var job in jobsList)
            {
                var title = job.FindElement(By.ClassName("jobTitle-link"));
                string titleText = title.GetAttribute("innerHTML");
                string link = title.GetAttribute("href");
                string department = job.FindElement(By.ClassName("jobFacility")).Text;
                string[] location = job.FindElement(By.XPath(".//span[@class='jobLocation']")).Text.Split(',');

                Job newJob = new Job();

                newJob.Title = titleText.ToString();
                newJob.Department = department.ToString();
                newJob.Region = location[1].Trim(' ');
                newJob.City = location[0];
                newJob.Company = company;
                newJob.Status = 0;
                newJob.InterviewRound = 0;

                jobsLink.Add(newJob);

            }

            await _repository.AddJobs(jobsLink);
        }


        public async Task<int> RunAutomationISS()
        {
            string company = "issworld";
            string fullUrl = "https://www.pl.issworld.com/kariera/oferty-pracy#skk-container";

            await scraper.GetHtmlAsync(fullUrl);

            int recordsUpdatedTotal = 0;

            var lastPagerEl = await scraper.WaitForElementAsync(By.XPath(".//a[@class='skk_pager_last']"));
            int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 1;

            // delete eveything to make sure everything is up to date
            await _repository.DeleteJobByCompany(company);

            await scrapeJobsISS(company);

            foreach (int _ in Enumerable.Range(1, lastPagerCount))
            {
                await scraper.ClickElementAsync(By.XPath(".//a[@class='skk_pager_next']"));
                await scrapeJobsISS(company);
            }

            return recordsUpdatedTotal;
        }

        public async Task<int> RunAutomationMacgregor()
        {
            string company = "macgregor";
            string fullUrl = "https://careers.macgregor.com/search";

            await scraper.GetHtmlAsync(fullUrl);

            int recordsUpdatedTotal = 0;

            var pagination = await scraper.WaitForElementsAsync(By.XPath(".//ul[@class='pagination']//li"));

            // delete eveything to make sure everything is up to date
            await _repository.DeleteJobByCompany(company);

            await scrapeJobsMacgregor(company);

            for (int i = 2; i < pagination.Count - 1; i++)
            {
                var page = await scraper.WaitForElementAsync(By.XPath($".//a[@title='Page {i}']"));
                await scraper.ClickElementAsync(page);
                await scrapeJobsMacgregor(company);
            }

            return recordsUpdatedTotal;
        }
    }
}
