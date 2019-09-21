using Ed.BudgetVisualizer.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Ed.BudgetVisualizer.Logic
{
    /// <summary>
    /// Transacton history parsing interface.
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
        /// Parses a file data stream and outputs a list of transactions.
        /// </summary>
        /// <param name="data">File data stream.</param>
        Task<List<Transaction>> ParseFile(Stream data);

        /// <summary>
        /// Determines each transaction's category from a list of available categories.
        /// </summary>
        /// <param name="transactions">Transaction list.</param>
        /// <param name="categories">Category list.</param>
        void UpdateTransactionCategories(List<Transaction> transactions, List<Category> categories);
    }
}
