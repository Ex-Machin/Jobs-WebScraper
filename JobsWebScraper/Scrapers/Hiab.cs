using JobsWebScraper.Services;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using JobsWebScraper.Models;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace JobsWebScraper.Scrapers
{
    class Hiab : IScraper
    {
        private List<Job> jobsLink = new List<Job>();

        private async Task ScrapePage(SeleniumScraper scraper)
        {
            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//table[@id='searchresults']//tbody//tr"));

            foreach (IWebElement job in jobsList)
            {
                var heading = job.FindElement(By.ClassName("jobTitle-link"));
                string title = heading.Text;
                string department = job.FindElement(By.ClassName("jobFacility")).Text ?? "";
                var city = job.FindElement(By.ClassName("jobLocation")).Text ?? "";
                string link = heading.GetAttribute("href") ?? "";
                string DatePreParced = job.FindElement(By.ClassName("jobDate")).Text;
                DateTime? datePublished = null;
                if (!DatePreParced.IsNullOrEmpty())
                {
                    datePublished = DateTime.Parse(DatePreParced);
                }
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

                this.jobsLink.Add(newJob);
            }
        }

        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper)
        {
            await scraper.GetHtmlAsync(url);

            // skip first active class and arrows elements
            var pagination = await scraper.WaitForElementsAsync(By.XPath(".//ul[@class='pagination']//li[not(class='active') and @class=' ']//a"));

            // after each click it updates the DOM, so we will cache the links
            var hrefs = pagination.Select(h => h.GetAttribute("href")).ToList();

            await ScrapePage(scraper);

            foreach (string href in hrefs)
            {
                await scraper.Navigate(href);

                await ScrapePage(scraper);
            }

            return jobsLink;
        }
    }
}