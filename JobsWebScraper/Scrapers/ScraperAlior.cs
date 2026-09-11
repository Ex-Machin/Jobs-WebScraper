using HtmlAgilityPack;
using JobsWebScraper.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Web;
using JobsWebScraper.Models;

namespace JobsWebScraper.Scrapers
{
    class ScraperMacgregor : IScraper
    {
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper) {
            string company = "aliorbank";

            await scraper.GetHtmlAsync(url);

            List<Job> jobsLink = new List<Job>();

            var lastPageContainer = await scraper.WaitForElementAsync(By.XPath(".//ul[@class='pagination']//li[@class='item'][last()]//a"));
            string lastPage = lastPageContainer.GetAttribute("innerHTML").Split(" ")[3];

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
                string workingType = "";

                Job newJob = new Job();

                createNewJob(newJob, title, department, city, company, link, datePublished, workingType);

                jobsLink.Add(newJob);
            }

            await _repository.AddJobs(jobsLink);

            for (int i = 1; i <= Int32.Parse(lastPage) - 1; i++)
            {
                await scraper.ClickElementAsync(await scraper.WaitForElementAsync(By.XPath(".//ul[@class='pagination']//li[@class='item next']//a")));
                await scrapeJobsAlior(company);
            }

            return jobsLink;
        }
    }
}