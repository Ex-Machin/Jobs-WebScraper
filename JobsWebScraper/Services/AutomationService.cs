using HtmlAgilityPack;
using JobsWebScraper.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Web;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class AutomationService : IAutomationService
    {
        private readonly IJobsRepository _repository;
        private static readonly SeleniumScraper scraper = new SeleniumScraper();

        public AutomationService(IJobsRepository repository)
        {
            _repository = repository;
        }

        private static string findElementWithPossibleNull(By by, IWebElement el)
        {
            try
            {
                return el.FindElement(by).Text;
            }
            catch
            {
                return "";
            }
        }

        private void createNewJob(Job newJob, string title, string department, string city, string company, string link, DateTime? datePublished)
        {
            newJob.Title = title;
            newJob.Department = department;
            newJob.City = city;
            newJob.Company = company;
            newJob.Link = link;
            newJob.DatePublished = datePublished;
        }

        private async Task scrapeJobsAlior(string company)
        {

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tbody//tr"));

            List<Job> jobsLink = new List<Job>();

            foreach (var job in jobsList)
            {
                var titleLink = job.FindElement(By.ClassName("job-link"));
                var title = titleLink.GetAttribute("innerHTML");
                string link = titleLink.GetAttribute("href");
                string department = findElementWithPossibleNull(By.XPath(".//td[@class='job-category']//span"), job);
                string city = job.FindElement(By.XPath(".//td[@class='job-location']//span")).Text;
                DateTime datePublished = DateTime.ParseExact(
                    job.FindElement(By.XPath(".//td[@class='job-date']//span")).Text,
                    "dd.MM.yyyy",
                    System.Globalization.CultureInfo.InvariantCulture
                );

                Job newJob = new Job();

                createNewJob(newJob, title, department, city, company, link, datePublished);

                jobsLink.Add(newJob);
            }

            await _repository.AddJobs(jobsLink);

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
                var city = columns[3].GetAttribute("textContent");
                string link = ""; // to extract
                DateTime? datePublished = null;

                Job newJob = new Job();

                createNewJob(newJob, title, department, city, company, link, datePublished);

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
                var titleLink = job.FindElement(By.ClassName("jobTitle-link"));
                var title = titleLink.GetAttribute("innerHTML");
                string link = titleLink.GetAttribute("href");
                string department = job.FindElement(By.ClassName("jobFacility")).Text;
                string[] location = job.FindElement(By.XPath(".//span[@class='jobLocation']")).Text.Split(',');
                string city = location[0];
                DateTime? datePublished = null;


                Job newJob = new Job();

                createNewJob(newJob, title, department, city, company, link, datePublished);

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
            int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 2;

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

        public async Task<int> RunAutomationAlior()
        {
            string company = "aliorbank";
            string fullUrl = "https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html";

            await scraper.GetHtmlAsync(fullUrl);

            int recordsUpdatedTotal = 0;

            var lastPageContainer = await scraper.WaitForElementAsync(By.XPath(".//ul[@class='pagination']//li[@class='item'][last()]//a"));
            string lastPage = lastPageContainer.GetAttribute("innerHTML").Split(" ")[3];

            // delete eveything to make sure everything is up to date
            await _repository.DeleteJobByCompany(company);

            await scrapeJobsAlior(company);

            for (int i = 1; i <= Int32.Parse(lastPage) - 1; i++)
            {
                await scraper.ClickElementAsync(await scraper.WaitForElementAsync(By.XPath(".//ul[@class='pagination']//li[@class='item next']//a")));
                await scrapeJobsAlior(company);
            }

            return recordsUpdatedTotal;
        }
    }
}
