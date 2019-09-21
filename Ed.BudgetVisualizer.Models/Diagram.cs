using System.Collections.Generic;

namespace Ed.BudgetVisualizer.Models
{
    /// <summary>
    /// Represents a data model for a Sankey diagram.
    /// </summary>
    public class Diagram
    {
        /// <summary>
        /// Diagram title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// List of links between nodes in the diagram.
        /// </summary>
        public List<LinkItem> Links { get; set; } = new List<LinkItem>();
    }
}
