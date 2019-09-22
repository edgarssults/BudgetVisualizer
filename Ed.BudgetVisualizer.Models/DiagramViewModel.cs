using System.Collections.Generic;

namespace Ed.BudgetVisualizer.Models
{
    /// <summary>
    /// Represents a view model for the diagram page.
    /// </summary>
    public class DiagramViewModel
    {
        /// <summary>
        /// Average monthly savings.
        /// </summary>
        public decimal MonthlySavingsAverage { get; set; }

        /// <summary>
        /// List of monthly diagrams.
        /// </summary>
        public List<Diagram> Diagrams { get; set; }
    }
}
