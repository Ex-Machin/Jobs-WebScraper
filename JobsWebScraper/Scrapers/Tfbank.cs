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
    class ScraperTfbank : IScraper
    {
        public string Company {get; set; }
        public ScraperTfbank(string company) 
        {
            this.Company = company;
        }
        public async Task<List<Job>> Scrape(string url, SeleniumScraper scraper) {
            await scraper.GetHtmlAsync(url);

            // Accept cookies
            await scraper.ClickElementAsync(By.XPath(".//button[@aria-label='Accept all cookies']"));

            // with span we check if button element is not empty, because
            // when clicking on all buttons - button element doesn't dissapear from the dom
            while (await scraper.isPresentInDom(By.XPath(".//div[@id='show_more_button']//span")))
            {
                var btn = await scraper.WaitForElementAsync(By.XPath(".//div[@id='show_more_button']"));
                await scraper.ClickElementAsync(btn.FindElement(By.TagName("a")));
            }

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//ul[@id='jobs_list_container']//li"));
            
            List<Job> jobsLink = new List<Job>();
            
            foreach (var job in jobsList)
            {
                try
                {
                    var title = job.FindElement(By.ClassName("text-block-base-link")).Text;
                    string? link = job.FindElement(By.TagName("a")).GetAttribute("href");
                    var descriptionBlock = job.FindElement(By.XPath(".//div[contains(@class, 'mt-1 text-md')]"));

                    string[] description = descriptionBlock.Text.Split('·');

                    string department = description[0].Trim();
                    string city = description[1].Trim();
                    string workingType = description.Length >= 3 ? description[2].Trim() : "";
                    DateTime? datePublished = null;

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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return jobsLink;
        }
    }
}