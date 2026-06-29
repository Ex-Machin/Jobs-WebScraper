using HtmlAgilityPack;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Services
{

    public class AutomationService : IAutomationService
    {
        private readonly IJobsRepository _repository;

        public AutomationService(IJobsRepository repository)
        {
            _repository = repository;
        }


        public async Task<int> RunAutomation()
        {
            //string url = "https://bluestonepim.bamboohr.com/careers/";
            //string url = $"https://kariera.comarch.pl/praca/"; 
            //string url = $"https://www.bgk.pl/kariera-w-bgk/oferty-pracy-i-praktyk/";

            string company = "issworld";
            string fullUrl = "https://www.pl.issworld.com/kariera/oferty-pracy";
            var options = new ChromeOptions()
            {
                BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };
            options.AddArguments(new List<string>() { "headless", "disable-gpu" });
            var browser = new ChromeDriver(options);
            browser.Navigate().GoToUrl(fullUrl);

            int recordsUpdatedTotal = 0;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(browser.PageSource);
            HtmlNodeCollection jobsList = htmlDoc.DocumentNode.SelectNodes(".//tr[@class='skk_row_odd'] | .//tr[@class='skk_row_even']");

            List<Job> currentJobs = await _repository.GetAllJobs();
            List<string> currentJobsTitles = new List<string>();

            foreach (Job job in currentJobs)
            {
                currentJobsTitles.Add(job.Title);
            }

            foreach (var job in jobsList)
            {
                var title = job.InnerText;
                if (!currentJobsTitles.Contains(title)) // optimize by getting only current company
                {
                    var department = job.ChildNodes[1].InnerText;
                    var region = job.ChildNodes[2].InnerText;
                    var city = job.ChildNodes[2].InnerText;

                    Job newJob = new Job();

                    newJob.Title = title.ToString();
                    newJob.Departement = department.ToString();
                    newJob.Region = region.ToString();
                    newJob.City = city.ToString();
                    newJob.Company = company;
                    newJob.Status = 0;
                    newJob.InterviewRound = 0;

                    await _repository.AddJob(newJob);
                    recordsUpdatedTotal++;
                }
            }

            return recordsUpdatedTotal;
        }
    }
}