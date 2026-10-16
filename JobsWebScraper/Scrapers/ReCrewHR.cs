using JobsWebScraper.Services;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using JobsWebScraper.Models;

namespace JobsWebScraper.Scrapers
{
    class ReCrewHR : IScraper
    {
        // not enough offers to check pagination logic
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper) {
            await scraper.GetHtmlAsync(url);

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//div[@class='list-data']"));
            
            List<Job> jobsLink = new List<Job>();
            
            foreach (var job in jobsList)
            {
                try
                {
                    var title = job.FindElement(By.ClassName("job-title")).Text;
                    string link = job.FindElement(By.TagName("a")).GetAttribute("href") ?? "";

                    string department = "";
                    string city = job.FindElement(By.ClassName("job-location")).Text;
                    string workingType = "";
                    DateTime? datePublished = null; // to extract

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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return jobsLink;
        }
    }
}