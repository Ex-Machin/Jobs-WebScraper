using JobsWebScraper.Services;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using JobsWebScraper.Models;

namespace JobsWebScraper.Scrapers
{
    class KlinikaBocian : IScraper
    {
        // Most probably this website doesn't have the pagination implemented yet
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper) {
            await scraper.GetHtmlAsync(url);

            // Deny cookies
            await scraper.ClickElementAsync(By.Id("CybotCookiebotDialogBodyButtonDecline"));

            List<Job> jobsLink = new List<Job>();

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.ClassName("offer__item"));

            foreach (IWebElement job in jobsList)
            {
                string title = job.FindElement(By.XPath(".//div[@class='erecruiter']/h3")).GetAttribute("innerHTML");
                string link = job.FindElement(By.XPath("//a[@target='play-frame']")).GetAttribute("href") ?? "";
                string department = "";
                string city = job.FindElement(By.XPath(".//div[@class='erecruiter']/p[contains(@class, 'region')]"))
                    .GetAttribute("textContent")
                    .Trim();
                DateTime? datePublished = null;
                string workingType = "";

                Job newJob = new Job
                {
                    Title = title,
                    Department = department,
                    City = city,
                    Company = this.GetType().Name,
                    Link = link,
                    DatePublished = datePublished,
                    WorkingType = workingType
                };

                jobsLink.Add(newJob);
            }

            return jobsLink;
        }
    }
}