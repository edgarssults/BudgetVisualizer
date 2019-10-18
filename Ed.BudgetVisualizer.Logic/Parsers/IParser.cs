using Ed.BudgetVisualizer.Models;

namespace Ed.BudgetVisualizer.Logic.Parsers
{
    /// <summary>
    /// Transaction history parsing interface.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Whether the transaction history has a title row.
        /// </summary>
        bool HasTitleRow { get; }

        /// <summary>
        /// Whether the transaction history has a header row.
        /// </summary>
        bool HasHeaderRow { get; }

        /// <summary>
        /// Column separator.
        /// </summary>
        char Separator { get; }

        /// <summary>
        /// Maps data fields to a transaction.
        /// </summary>
        /// <param name="fields">Data fields to map.</param>
        Transaction MapTransaction(string[] fields);
    }
}
