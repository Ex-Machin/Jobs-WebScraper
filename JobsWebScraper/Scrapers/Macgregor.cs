using JobsWebScraper.Services;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using JobsWebScraper.Models;

namespace JobsWebScraper.Scrapers
{
    class Macgregor : IScraper
    {
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper)
        {
            await scraper.GetHtmlAsync(url);

            var pagination = await scraper.WaitForElementsAsync(By.XPath(".//ul[@class='pagination']//li"));

            List<Job> jobsLink = new List<Job>();

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='data-row']"));

            foreach (var job in jobsList)
            {
                var titleLink = job.FindElement(By.ClassName("jobTitle-link"));
                var title = titleLink.GetAttribute("innerHTML");
                string link = titleLink.GetAttribute("href") ?? "";
                string department = job.FindElement(By.ClassName("jobFacility")).Text;
                string[] location = job.FindElement(By.XPath(".//span[@class='jobLocation']")).Text.Split(',');
                string city = location[0];
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

            for (int i = 2; i < pagination.Count - 1; i++)
            {
                var page = await scraper.WaitForElementAsync(By.XPath($".//a[@title='Page {i}']"));
                await scraper.ClickElementAsync(page);
                jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='data-row']"));

                foreach (var job in jobsList)
                {
                    var titleLink = job.FindElement(By.ClassName("jobTitle-link"));
                    var title = titleLink.GetAttribute("innerHTML");
                    string link = titleLink.GetAttribute("href") ?? "";
                    string department = job.FindElement(By.ClassName("jobFacility")).Text;
                    string[] location = job.FindElement(By.XPath(".//span[@class='jobLocation']")).Text.Split(',');
                    string city = location[0];
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
            }

            return jobsLink;
        }
    }
}