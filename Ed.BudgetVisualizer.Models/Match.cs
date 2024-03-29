namespace Ed.BudgetVisualizer.Models
{
    public class Match
    {
        /// <summary>
        /// Category identifier.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Regex pattern to use for matching the origin text.
        /// </summary>
        public string OriginRegex { get; set; }

        /// <summary>
        /// Regex pattern to use for matching the description text.
        /// </summary>
        public string DescriptionRegex { get; set; }
    }
}
