using JobsWebScraper.Models;

namespace JobsWebScraper.Services 
{
    interface IScraper
    {
        string Company {get; set; }
        Task<List<Job>> Scrape(string url, SeleniumScraper scraper);
    }
}