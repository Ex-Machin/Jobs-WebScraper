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
    class KlinikaBocian : IScraper
    {
        public string Company { get; set;}
        public KlinikaBocian(string company) 
        {
            this.Company = company;
        }
        // Most probably this website doesn't have the pagination implemented yet
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper) {
            await scraper.GetHtmlAsync(url);

            List<Job> jobsLink = new List<Job>();

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.ClassName("offer"));

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

                Job newJob = new Job();

                newJob.Title = title;
                newJob.Department = department;
                newJob.City = city;
                newJob.Company = this.Company;
                newJob.Link = link;
                newJob.DatePublished = datePublished;
                newJob.WorkingType = workingType;

                jobsLink.Add(newJob);
            }

            return jobsLink;
        }
    }
}