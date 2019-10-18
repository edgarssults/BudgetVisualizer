using Ed.BudgetVisualizer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ed.BudgetVisualizer.Logic
{
    /// <summary>
    /// Data extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Transforms a list of transactions to a diagram model.
        /// </summary>
        /// <param name="transactions">Transaction list.</param>
        public static DiagramViewModel ToDiagramModel(this List<Transaction> transactions)
        {
            var diagrams = new List<Diagram>();
            var savings = new List<decimal>();

            // Group months
            var months = transactions
                .Where(t => !t.IsExcluded)
                .OrderByDescending(t => t.Date)
                .GroupBy(t => $"{t.Date.Month}/{t.Date.Year}")
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var month in months)
            {
                var diagram = new Diagram
                {
                    Title = month.Key,
                };

                // Group categories
                var categories = month.Value
                    .GroupBy(t => t.Category)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var category in categories)
                {
                    decimal categoryCreditSum = category.Value.Where(t => t.IsCredit).Sum(c => c.Sum);
                    if (categoryCreditSum > 0)
                    {
                        diagram.Links.Add(new LinkItem
                        {
                            From = category.Key,
                            To = "Budget",
                            Value = categoryCreditSum,
                        });
                    }

                    decimal categoryDebitSum = category.Value.Where(t => t.IsDebit).Sum(c => c.Sum);
                    if (categoryDebitSum > 0)
                    {
                        diagram.Links.Add(new LinkItem
                        {
                            From = "Budget",
                            To = category.Key,
                            Value = categoryDebitSum,
                        });
                    }
                }

                // Detect duplicates
                foreach (var link in diagram.Links.Where(l => l.To == "Budget" && diagram.Links.Any(d => d.To == l.From)))
                {
                    link.From += " +";
                };
                foreach (var link in diagram.Links.Where(l => l.From == "Budget" && diagram.Links.Any(d => d.From == l.To)))
                {
                    link.To += " -";
                };

                // Sort the links by value
                diagram.Links = diagram.Links.OrderByDescending(l => l.Value).ToList();

                // Calculate savings
                decimal monthCreditSum = month.Value.Where(t => t.IsCredit).Sum(c => c.Sum);
                decimal monthDebitSum = month.Value.Where(t => t.IsDebit).Sum(c => c.Sum);
                decimal diff = monthCreditSum - monthDebitSum;
                if (diff > 0)
                {
                    diagram.Links.Insert(0, new LinkItem
                    {
                        From = "Budget",
                        To = "Savings",
                        Value = diff,
                    });
                    savings.Add(diff);
                }
                else if (diff < 0)
                {
                    diagram.Links.Add(new LinkItem
                    {
                        From = "Debt",
                        To = "Budget",
                        Value = diff * -1,
                    });
                    savings.Add(0);
                }
                else
                {
                    savings.Add(0);
                }

                diagrams.Add(diagram);
            }

            return new DiagramViewModel
            {
                Diagrams = diagrams,
                MonthlySavingsAverage = savings.Average(),
            };
        }

        /// <summary>
        /// Determines whether the IEnumerable is not null and not empty.
        /// </summary>
        /// <param name="obj">The IEnumerable to check.</param>
        /// <returns>True if IEnumerable isn't null or empty.</returns>
        public static bool IsAny(this IEnumerable<object> obj)
        {
            return obj != null && obj.Any();
        }
    }
}
