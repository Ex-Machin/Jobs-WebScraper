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
    class ScraperISS : IScraper
    {
        public string Company {get; set;}
        public ScraperISS(string company) 
        {
            this.Company = company;
        }
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper) {
            await scraper.GetHtmlAsync(url);

            // remove parent of shadow DOM element to unblock the view
            await scraper.removeElementFromDOM(By.Id("usercentrics-cmp-ui"));

            var lastPagerEl = await scraper.WaitForElementAsync(By.XPath(".//a[@class='skk_pager_last']"));
            int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 1;

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

            foreach (int _ in Enumerable.Range(1, lastPagerCount))
            {
                await scraper.ClickElementAsync(By.XPath(".//a[@class='skk_pager_next']"));
                jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']"));

                foreach (var job in jobsList)
                {
                    var columns = job.FindElements(By.TagName("td"));
                    string title = columns[0].GetAttribute("textContent");
                    var department = columns[1].GetAttribute("textContent");
                    var city = columns[3].GetAttribute("textContent");
                    string link = ""; // to extract
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
            }

            return jobsLink;
        }
    }
}