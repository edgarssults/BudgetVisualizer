namespace Ed.BudgetVisualizer.Models
{
    /// <summary>
    /// Represents a data link in a Sankey diagram.
    /// </summary>
    public class LinkItem
    {
        /// <summary>
        /// Link source.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Link value.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Link target.
        /// </summary>
        public string To { get; set; }
    }
}
