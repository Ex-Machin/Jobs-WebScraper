using Microsoft.AspNetCore.Mvc;
using JobsWebScraper.Services;
using JobsWebScraper.Scrapers;
using JobsWebScraper.Models;
using JobsWebScraper.Data;
using Microsoft.EntityFrameworkCore;

namespace JobsWebScraper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutomationController : ControllerBase
    {
        private readonly MyAPIContext _context;
        public AutomationController(MyAPIContext context)
        {
            _context = context;
        }
        [HttpPost("all")]
        public async Task<IActionResult> Post()
        {
            SeleniumScraper scraperInstance = new SeleniumScraper();

            var scrapers = new Dictionary<string, IScraper>();
            // scrapers.Add("https://careers.macgregor.com/search", new Macgregor());
            // scrapers.Add("https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html", new Alior());
            // scrapers.Add("https://www.pl.issworld.com/kariera/oferty-pracy#skk-container", new ISS());
            // scrapers.Add("https://tfbank.teamtailor.com/jobs", new Tfbank());
            // scrapers.Add("https://klinikabocian.pl/klinika/kariera", new KlinikaBocian());
            // scrapers.Add("https://recrewhr.pl/oferty-pracy/", new ReCrewHR());
            // scrapers.Add("https://careers.marsh.com/eu/en/poland-warsaw-opportunities/", new Marsh());

            // var websitesForScraping = db.LoadWebsites();

            List<string> websitesForScraping = new List<string>  {
                // "https://careers.macgregor.com/search",
                // "https://www.aliorbank.pl/dodatkowe-informacje/kariera/aktualne-oferty-pracy.html",
                // "https://www.pl.issworld.com/kariera/oferty-pracy#skk-container",
                // "https://tfbank.teamtailor.com/jobs",
                // "https://klinikabocian.pl/klinika/kariera",
                "https://recrewhr.pl/oferty-pracy/"
            };

            foreach (string website in websitesForScraping)
            {
                var scraper = scrapers[website];

                List<Job> jobs = await scraper.Scrape(website, scraperInstance);

                // delete eveything to make sure everything is up to date
                await _context.Job.Where(j => j.Company == scraper.GetType().Name).ExecuteDeleteAsync();
                _context.Job.AddRange(jobs);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
