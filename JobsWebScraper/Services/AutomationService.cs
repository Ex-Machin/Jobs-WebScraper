using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Web;
using TaskManager.Models;
//using TaskManager.Services;
//using AngleSharp.Dom;
//using HtmlAgilityPack;
//using OpenQA.Selenium;
//using System.Collections.ObjectModel;
//using WebScraping_testing.Models;
//using WebScraping_testing.Services;


namespace TaskManager.Services
{
    public class SeleniumScraper : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly Actions _actions;
        private readonly IJavaScriptExecutor _ex;

        public SeleniumScraper()
        {
            // close previous sessions (if any)
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"no previous sessions: {ex.ToString()}");
            }

            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Run Chrome in headless mode (optional)
            _driver = new ChromeDriver(options);
            _actions = new Actions(_driver);
            _ex = (IJavaScriptExecutor)_driver;
        }

        public async Task<string> GetHtmlAsync(string url)
        {
            await Task.Run(() => _driver.Navigate().GoToUrl(url));
            return _driver.PageSource;
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

        public async Task<ReadOnlyCollection<IWebElement>> WaitForElementsAsync(By by, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
            return await Task.Run(() => wait.Until(d => d.FindElements(by)));
        }
        public async Task<IWebElement> WaitForElementAsync(By by, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
            var el = await Task.Run(() => wait.Until(d => d.FindElement(by)));
            return el;
        }

        public async Task ClickElementAsync(By by, int timeoutSeconds = 10)
        {
            var element = await WaitForElementAsync(by, timeoutSeconds);
            _ex.ExecuteScript("arguments[0].click();", element);
            _actions.MoveToElement(element).Click().Perform();
            Thread.Sleep(5000);

        }

        public async Task SendKeysAsync(By by, string text)
        {
            var element = await WaitForElementAsync(by);
            await Task.Run(() => element.SendKeys(text));
        }
    }

    public class AutomationService : IAutomationService
    {
        private readonly IJobsRepository _repository;
        private static readonly SeleniumScraper scraper = new SeleniumScraper();

        public AutomationService(IJobsRepository repository)
        {
            _repository = repository;
        }

        private async Task scrapeJobs(string company)
        {

            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']"));
            Console.WriteLine(jobsList);

            List<Job> jobsLink = new List<Job>();

            List<Job> currentJobs = await _repository.GetAllJobs();
            List<string> currentJobsTitles = new List<string>();

            foreach (Job job in currentJobs)
            {
                currentJobsTitles.Add(job.Title);
            }

            foreach (var job in jobsList)
            {
                var columns = job.FindElements(By.TagName("td"));
                string title = columns[0].GetAttribute("textContent");

                if (!currentJobsTitles.Contains(title)) // optimize by getting only current company
                {
                    var department = columns[1].GetAttribute("textContent");
                    var region = columns[2].GetAttribute("textContent");
                    var city = columns[3].GetAttribute("textContent");

                    Job newJob = new Job();

                    newJob.Title = title.ToString();
                    newJob.Departement = department.ToString();
                    newJob.Region = region.ToString();
                    newJob.City = city.ToString();
                    newJob.Company = company;
                    newJob.Status = 0;
                    newJob.InterviewRound = 0;

                    await _repository.AddJob(newJob);
                }


            }

        }


        public async Task<int> RunAutomation()
        {
            string company = "issworld";
            string fullUrl = "https://www.pl.issworld.com/kariera/oferty-pracy#skk-container";

            await scraper.GetHtmlAsync(fullUrl);

            int recordsUpdatedTotal = 0;

            var lastPagerEl = await scraper.WaitForElementAsync(By.XPath(".//a[@class='skk_pager_last']"));
            int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 1;

            await scrapeJobs(company);

            foreach (int _ in Enumerable.Range(1, lastPagerCount))
            {
                await scraper.ClickElementAsync(By.XPath(".//a[@class='skk_pager_next']"));
                await scrapeJobs(company);
            }

            return recordsUpdatedTotal;

            //HtmlDocument htmlDoc = new HtmlDocument();
            //htmlDoc.LoadHtml(browser.PageSource);
            //HtmlNodeCollection jobsList = htmlDoc.DocumentNode.SelectNodes(".//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']");

            //var lastPagerEl = await scraper.WaitForElementAsync(By.XPath(".//a[@class='skk_pager_last']"));
            //int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 1;
            //await scrapeJobs();

            //List<Job> currentJobs = await _repository.GetAllJobs();
            //List<string> currentJobsTitles = new List<string>();

            //foreach (Job job in currentJobs)
            //{
            //    currentJobsTitles.Add(job.Title);
            //}

            //foreach (var job in jobsList)
            //{
            //    var title = job.InnerText;
            //    if (!currentJobsTitles.Contains(title)) // optimize by getting only current company
            //    {
            //        var department = job.ChildNodes[1].InnerText;
            //        var region = job.ChildNodes[2].InnerText;
            //        var city = job.ChildNodes[2].InnerText;

            //        Job newJob = new Job();

            //        newJob.Title = title.ToString();
            //        newJob.Departement = department.ToString();
            //        newJob.Region = region.ToString();
            //        newJob.City = city.ToString();
            //        newJob.Company = company;
            //        newJob.Status = 0;
            //        newJob.InterviewRound = 0;

            //        await _repository.AddJob(newJob);
            //        recordsUpdatedTotal++;
            //    }
            //}

            //return recordsUpdatedTotal;
        }
    }
}

//namespace App
//{
//    public class App
//    {

//        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
//        //static readonly HttpClient client = new HttpClient();

//        static readonly SeleniumScraper scraper = new SeleniumScraper();

//        private static async Task scrapeJobs()
//        {

//            ReadOnlyCollection<IWebElement> jobsList = await scraper.WaitForElementsAsync(By.XPath(".//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']"));
//            Console.WriteLine(jobsList);

//            List<JobItem> jobsLink = new List<JobItem>();

//            foreach (var job in jobsList)
//            {
//                try
//                {
//                    var jobHTML = job.GetAttribute("outerHTML");
//                    Console.WriteLine(jobHTML);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex);
//                }

//                //var title = job.InnerText;
//                //var department = job.ChildNodes[1].InnerText;
//                //var region = job.ChildNodes[2].InnerText;
//                //var city = job.ChildNodes[2].InnerText;

//                ////var url = link.SelectSingleNode(".//span[@class='titleline']").FirstChild.GetAttributeValue("href", string.Empty);
//                //JobItem item = new JobItem();
//                //item.title = title.ToString();
//                //item.department = department.ToString();
//                //item.region = region.ToString();
//                //item.city = city.ToString();
//                //jobsLink.Add(item);
//            }
//            //string results = JsonConvert.SerializeObject(jobsLink);
//            //return results;
//        }

//        //private static string ParseHtml(string html)
//        //{
//        //    HtmlDocument htmlDoc = new HtmlDocument();
//        //    htmlDoc.LoadHtml(html);

//        //    HtmlNode lastPagerEl = htmlDoc.DocumentNode.SelectNodes(".//a[@class='skk_pager_last']")[0];

//        //    Console.WriteLine(lastPagerEl.InnerText);
//        //    // scrape first page
//        //    scrapeJobs(htmlDoc);

//        //    //foreach (int i in Enumerable.Range(2, Int32.Parse(lastPagerEl.InnerText)))
//        //    //{
//        //    //    Console.WriteLine(i);
//        //    //}
//        //    int j = 2;
//        //    //var PagerEl = htmlDoc.DocumentNode.SelectNodes($".//a[contains(@class, 'skk_pager_page_{j}')]")[0];
//        //    var PagerEl = htmlDoc.DocumentNode.Descendants("a")
//        //    .Where(node => node.GetAttributeValue("class", "").Contains($"skk_pager_page_{j}")).ToList();

//        //    var btnele = (HTMLButtonElement)ele.DomElement;

//        //    htmlDoc.



//        //    return "testing";

//        //}
//        static async Task Main()
//        {
//            // Call asynchronous network methods in a try/catch block to handle exceptions.
//            //try
//            //{
//            //    string fullUrl = "https://www.pl.issworld.com/kariera/oferty-pracy";
//            //    var options = new ChromeOptions()
//            //    {
//            //        BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
//            //    };
//            //    options.AddArguments(new List<string>() { "headless", "disable-gpu" });
//            //    var browser = new ChromeDriver(options);
//            //    browser.Navigate().GoToUrl(fullUrl);
//            //    var linkList = ParseHtml(browser.PageSource);
//            //    Console.WriteLine(linkList);
//            //}
//            //catch (HttpRequestException e)
//            //{
//            //    Console.WriteLine("\nException Caught!");
//            //    Console.WriteLine("Message :{0} ", e.Message);
//            //}

//            //var scraper = new WebScraper(TimeSpan.FromSeconds(30));


//            //string url = "https://www.scrapingcourse.com/ecommerce/";
//            //var scraper = new WebScraper();
//            //var res = await scraper.GetHtmlAsync("https://www.pl.issworld.com/kariera/oferty-pracy");

//            // getting by xpath
//            //var html = await scraper.GetHtmlAsync(url);
//            //var document = scraper.ParseHtml(html);
//            //var titles = scraper.ExtractTextWithXPath(document, "//h1");

//            //Console.WriteLine(titles[0]);

//            // getting child elements

//            //var listItems = scraper.GetChildNodes(document, "//ul[@class='menu']");
//            //foreach (var item in listItems)
//            //{
//            //    Console.WriteLine(item.InnerText.Trim());
//            //}

//            // extract attributes
//            //var links = scraper.ExtractAttributeValues(document, "//a", "href");

//            // iterating through products
//            //var res = await ScrapeProductListing(url, document);

//            // selenium scraper

//            await scraper.GetHtmlAsync("https://www.pl.issworld.com/kariera/oferty-pracy#skk-container");

//            // getting by xpath
//            //var html = await scraper.GetHtmlAsync(url);
//            //var document = scraper.ParseHtml(html);

//            //var element = await scraper.WaitForElementAsync(By.ClassName("skk_pager_last"));
//            //Console.WriteLine(element.Text);

//            var lastPagerEl = await scraper.WaitForElementAsync(By.XPath(".//a[@class='skk_pager_last']"));
//            int lastPagerCount = Int32.Parse(lastPagerEl.Text) - 1;
//            await scrapeJobs();

//            foreach (int _ in Enumerable.Range(1, lastPagerCount))
//            {
//                await scraper.ClickElementAsync(By.XPath(".//a[@class='skk_pager_next']"));
//                //await scrapeJobs();
//            }
//            //await scraper.ClickElementAsync(By.ClassName("skk_pager_last"));
//            //scrapeJobs(scraper);

//            //ReadOnlyCollection<IWebElement> elements = await scraper.WaitForElementsAsync(By.XPath("//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']"));



//        }

//    }
//}

