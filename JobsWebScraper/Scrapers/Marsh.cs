using JobsWebScraper.Services;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using JobsWebScraper.Models;

namespace JobsWebScraper.Scrapers
{
    class Marsh : IScraper
    {
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper)
        {
            await scraper.GetHtmlAsync(url);

            // accept cookies
            await scraper.ClickElementAsync(By.ClassName("osano-cm-accept-all"));

            // clear default filters
            await scraper.ClickElementAsync(By.XPath(".//ppc-content[@key='clearAllFacets']"));


            List<Job> jobsLink = new List<Job>();

            while (await scraper.isPresentInDom(By.XPath(".//a[@aria-label='View next page']")))
            {

                ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//li[contains(@class, 'jobs-list-item')]"));

                foreach (IWebElement job in jobsList)
                {
                    var heading = job.FindElement(By.XPath(".//span[@role='heading']"));
                    string title = heading.FindElement(By.ClassName("au-target")).Text;
                    string department = job.FindElement(By.ClassName("job-category")).Text.Split('\n').Last();
                    var city = job.FindElement(By.ClassName("job-location")).Text.Split('\n').Last();
                    string link = heading.FindElement(By.TagName("a")).GetAttribute("href") ?? "";
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

                if (await scraper.isPresentInDom(By.XPath(".//a[contains(@class, 'aurelia-hide') and @aria-label='View next page']")))
                {
                    break;
                }

                await scraper.ClickElementAsync(By.XPath(".//a[@aria-label='View next page']"));
            }


            return jobsLink;
        }
    }
}