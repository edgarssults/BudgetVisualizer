using System;

namespace Ed.BudgetVisualizer.Models
{
    /// <summary>
    /// Represents a transaction imported from a transaction history data file.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Transaction date (not the same as transaction initiation date).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Transaction description (notes).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Transaction origin (name of the company or person initiating it).
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Whether it is a debit transaction.
        /// </summary>
        public bool IsDebit { get; set; }

        /// <summary>
        /// Whether it is a credt transaction.
        /// </summary>
        public bool IsCredit { get; set; }

        /// <summary>
        /// Transaction sum in the account's currency.
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// Transaction currency (same as the account's currency).
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Whether the transacrion is excluded from diagrams.
        /// </summary>
        public bool IsExcluded { get; set; }

        /// <summary>
        /// Transaction category identifier.
        /// </summary>
        public int CategoryId { get; set; }
    }
}
