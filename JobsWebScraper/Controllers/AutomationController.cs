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

        // private readonly IAutomationService _automation;
        private readonly IJobsRepository _repository;
        // // public AutomationController(IAutomationService automation)
        public AutomationController(IJobsRepository repository)
        {
        //     // _automation = automation;
            _repository = repository;
        }
        [HttpPost("all")]
        public async Task<IActionResult> Post()
        {
            // await _automation.RunAutomationISS();
            // await _automation.RunAutomationAlior();
            // await _automation.RunAutomationTfbank();

            SeleniumScraper scraperInstance = new SeleniumScraper();

            var scrapers = new Dictionary<string, IScraper>();
            scrapers.Add("https://careers.macgregor.com/search", new ScraperMacgregor());
            scrapers.Add("https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html", new ScraperAlior());

            // var websitesForScraping = db.LoadWebsites();
            var websitesForScraping = new Dictionary<string, string>();
            websitesForScraping.Add("https://careers.macgregor.com/search", "macgregor");
            websitesForScraping.Add("https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html", "alior");

            foreach(KeyValuePair<string, string> website  in websitesForScraping) {
                var scraper = scrapers[website.Key];

                List<Job> jobs = await scraper.Scrape(website.Key, scraperInstance);
                // delete eveything to make sure everything is up to date
                await _repository.DeleteJobByCompany(website.Value);
                await _repository.AddJobs(jobs);
            }

            return Ok();
        }
    }
}
 