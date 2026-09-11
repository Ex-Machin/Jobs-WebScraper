using JobsWebScraper.Models;

namespace JobsWebScraper.Services 
{
    interface IScraper
    {
        Task<List<Job>> Scrape(string url, SeleniumScraper scraper);
    }
}