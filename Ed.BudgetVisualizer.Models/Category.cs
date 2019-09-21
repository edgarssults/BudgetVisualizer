using System;

namespace Ed.BudgetVisualizer.Models
{
    public class Category
    {
        /// <summary>
        /// Category display name.
        /// </summary>
        public string Name { get; set; }

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
