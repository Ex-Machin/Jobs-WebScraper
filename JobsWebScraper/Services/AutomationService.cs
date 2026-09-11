using HtmlAgilityPack;
using JobsWebScraper.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Web;
using JobsWebScraper.Models;

namespace JobsWebScraper.Services
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

        private void createNewJob(Job newJob, string title, string department, string city, string company, string link, DateTime? datePublished, string workingType)
        {
            newJob.Title = title;
            newJob.Department = department;
            newJob.City = city;
            newJob.Company = company;
            newJob.Link = link;
            newJob.DatePublished = datePublished;
            newJob.WorkingType = workingType;
        }

        // private async Task scrapeJobsAlior(string company)
        // {
        //     ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tbody//tr"));

        //     List<Job> jobsLink = new List<Job>();

        //     foreach (var job in jobsList)
        //     {
        //         var titleLink = job.FindElement(By.ClassName("job-link"));
        //         var title = titleLink.GetAttribute("innerHTML");
        //         string link = titleLink.GetAttribute("href");
        //         string department = findElementWithPossibleNull(By.XPath(".//td[@class='job-category']//span"), job);
        //         string city = job.FindElement(By.XPath(".//td[@class='job-location']//span")).Text;
        //         DateTime datePublished = DateTime.ParseExact(
        //             job.FindElement(By.XPath(".//td[@class='job-date']//span")).Text,
        //             "dd.MM.yyyy",
        //             System.Globalization.CultureInfo.InvariantCulture
        //         );
        //         string workingType = "";

        //         Job newJob = new Job();

        //         createNewJob(newJob, title, department, city, company, link, datePublished, workingType);

        //         jobsLink.Add(newJob);
        //     }

        //     await _repository.AddJobs(jobsLink);

        // }

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
                string workingType = "";

                Job newJob = new Job();

                createNewJob(newJob, title, department, city, company, link, datePublished, workingType);

                jobsLink.Add(newJob);
            }

            await _repository.AddJobs(jobsLink);

        }

        private async Task scrapeJobsTfbank(string company)
        {
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

                    createNewJob(newJob, title, department, city, company, link, datePublished, workingType);

                    jobsLink.Add(newJob);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }


            await _repository.AddJobs(jobsLink);
        }


        public async Task RunAutomationISS()
        {
            string company = "issworld";
            string fullUrl = "https://www.pl.issworld.com/kariera/oferty-pracy#skk-container";

            await scraper.GetHtmlAsync(fullUrl);

            // remove parent of shadow DOM element to unblock the view
            await scraper.removeElementFromDOM(By.Id("usercentrics-cmp-ui"));

            var lastPagerEl = await scraper.WaitForElementAsync(By.XPath(".//a[@class='skk_pager_last']"));
            int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 1;

            // delete eveything to make sure everything is up to date
            await _repository.DeleteJobByCompany(company);

            await scrapeJobsISS(company);

            foreach (int _ in Enumerable.Range(1, lastPagerCount))
            {
                await scraper.ClickElementAsync(By.XPath(".//a[@class='skk_pager_next']"));
                await scrapeJobsISS(company);
            }
        }

        // public async Task RunAutomationMacgregor()
        // {
        //     string company = "macgregor";
        //     string fullUrl = "https://careers.macgregor.com/search";

        //     await scraper.GetHtmlAsync(fullUrl);

        //     var pagination = await scraper.WaitForElementsAsync(By.XPath(".//ul[@class='pagination']//li"));

        //     // delete eveything to make sure everything is up to date
        //     await _repository.DeleteJobByCompany(company);

        //     await scrapeJobsMacgregor(company);

        //     for (int i = 2; i < pagination.Count - 1; i++)
        //     {
        //         var page = await scraper.WaitForElementAsync(By.XPath($".//a[@title='Page {i}']"));
        //         await scraper.ClickElementAsync(page);
        //         await scrapeJobsMacgregor(company);
        //     }
        // }

        // public async Task RunAutomationAlior()
        // {
        //     string company = "aliorbank";
        //     string fullUrl = "https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html";

        //     await scraper.GetHtmlAsync(fullUrl);

        //     var lastPageContainer = await scraper.WaitForElementAsync(By.XPath(".//ul[@class='pagination']//li[@class='item'][last()]//a"));
        //     string lastPage = lastPageContainer.GetAttribute("innerHTML").Split(" ")[3];

        //     // delete eveything to make sure everything is up to date
        //     await _repository.DeleteJobByCompany(company);

        //     await scrapeJobsAlior(company);

        //     for (int i = 1; i <= Int32.Parse(lastPage) - 1; i++)
        //     {
        //         await scraper.ClickElementAsync(await scraper.WaitForElementAsync(By.XPath(".//ul[@class='pagination']//li[@class='item next']//a")));
        //         await scrapeJobsAlior(company);
        //     }
        // }

        public async Task RunAutomationTfbank()
        {
            string company = "tfbank";
            string fullUrl = "https://tfbank.teamtailor.com/jobs";

            // delete eveything to make sure everything is up to date
            await _repository.DeleteJobByCompany(company);

            await scraper.GetHtmlAsync(fullUrl);

            // Accept cookies
            await scraper.ClickElementAsync(By.XPath(".//button[@aria-label='Accept all cookies']"));

            // delete eveything to make sure everything is up to date
            await _repository.DeleteJobByCompany(company);

            // with span we check if button element is not empty, because
            // when clicking on all buttons - button element doesn't dissapear from the dom
            while (await scraper.isPresentInDom(By.XPath(".//div[@id='show_more_button']//span")))
            {
                var btn = await scraper.WaitForElementAsync(By.XPath(".//div[@id='show_more_button']"));
                await scraper.ClickElementAsync(btn.FindElement(By.TagName("a")));
            }

            await scrapeJobsTfbank(company);
        }
    }
}
