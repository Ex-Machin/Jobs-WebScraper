using JobsWebScraper.Models;

namespace JobsWebScraper.Web.Services
{
    /// <summary>
    /// Presentation helpers for <see cref="Job"/>. These live in the UI project rather than on
    /// the shared model, which stays a plain data type owned by JobsWebScraper.Data.
    /// </summary>
    public static class JobExtensions
    {
        /// <summary>Filename of the company logo, e.g. "aliorbank" -> "aliorbank.svg".</summary>
        public static string LogoFileName(this Job job) => $"{job.Company}.svg";
    }
}
