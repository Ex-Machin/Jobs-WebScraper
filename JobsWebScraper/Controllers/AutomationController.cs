using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using JobsWebScraper.Services;
using JobsWebScraper.Scrapers;
using JobsWebScraper.Models;

namespace JobsWebScraper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutomationController : ControllerBase
    {
        private readonly IJobsRepository _repository;
        public AutomationController(IJobsRepository repository)
        {
            _repository = repository;
        }
        [HttpPost("all")]
        public async Task<IActionResult> Post()
        {
            SeleniumScraper scraperInstance = new SeleniumScraper();

            var scrapers = new Dictionary<string, IScraper>();
            scrapers.Add("https://careers.macgregor.com/search", new ScraperMacgregor("macgregor"));
            scrapers.Add("https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html", new ScraperAlior("aliorbank"));
            scrapers.Add("https://www.pl.issworld.com/kariera/oferty-pracy#skk-container", new ScraperISS("issworld"));
            scrapers.Add("https://tfbank.teamtailor.com/jobs", new ScraperTfbank("tfbank"));

            // var websitesForScraping = db.LoadWebsites();

            List<string> websitesForScraping = new List<string>  {
                "https://careers.macgregor.com/search",
                "https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html",
                "https://www.pl.issworld.com/kariera/oferty-pracy#skk-container",
                "https://tfbank.teamtailor.com/jobs"
            };

            foreach(string website in websitesForScraping) {
                var scraper = scrapers[website];

                List<Job> jobs = await scraper.Scrape(website, scraperInstance);

                // delete eveything to make sure everything is up to date
                await _repository.DeleteJobByCompany(scraper.Company);
                await _repository.AddJobs(jobs);
            }

            return Ok();
        }
    }
}
 