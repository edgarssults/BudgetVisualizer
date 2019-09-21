using Ed.BudgetVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ed.BudgetVisualizer.Logic
{
    /// <summary>
    /// SEB bank transaction history parsing implementation.
    /// </summary>
    public class SebParser : IParser
    {
        /// <summary>
        /// Whether the transaction history has a title row.
        /// </summary>
        public bool HasTitleRow => true;

        /// <summary>
        /// Whether the transaction history has a header row.
        /// </summary>
        public bool HasHeaderRow => true;

        /// <summary>
        /// Column separator.
        /// </summary>
        public char Separator => ';';

        /// <summary>
        /// Parses a file data stream and outputs a list of transactions.
        /// </summary>
        /// <param name="data">File data stream.</param>
        public async Task<List<Transaction>> ParseFile(Stream data) 
        {
            // TODO: Exception feedback to UI

            var transactions = new List<Transaction>();
            int lineNumber = 0;
            string line;

            using (StreamReader reader = new StreamReader(data))
            {
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (HasTitleRow && lineNumber == 1)
                    {
                        continue;
                    }

                    if (HasHeaderRow && lineNumber == 1)
                    {
                        continue;
                    }

                    if (HasTitleRow && HasHeaderRow && lineNumber == 2)
                    {
                        continue;
                    }

                    try
                    {
                        string[] fields = SplitAndClean(line);
                        var transaction = new Transaction
                        {
                            Date = DateTime.Parse(fields[1]),
                            Origin = fields[4],
                            Description = fields[9],
                            IsDebit = fields[14] == "D",
                            IsCredit = fields[14] == "C",
                            Sum = decimal.Parse(fields[15]),
                            Currency = fields[17],
                        };
                        transaction.Category = transaction.IsCredit ? "Other Income" : "Other Expense";
                        transactions.Add(transaction);
                    }
                    catch (ApplicationException ex)
                    {
                        Debug.WriteLine($"Could not parse line '{line}': {ex.Message}");
                    }
                }
            }

            return transactions;
        }

        /// <summary>
        /// Splits a line into fields and cleans each field from surrounding double quotes.
        /// </summary>
        /// <param name="line">Line to split and clean.</param>
        private string[] SplitAndClean(string line)
        {
            var fields = new List<string>();

            // Match either fields surrounded in quotes or fields without quotes
            // Both fields end with the separator
            foreach (Match match in Regex.Matches(line, $"(?:\"(.*?)\"|(\\d+.*?)){Separator}"))
            {
                // Add whichever group matched
                fields.Add(!string.IsNullOrEmpty(match.Groups[1].ToString())
                    ? match.Groups[1].ToString().Trim()
                    : match.Groups[2].ToString().Trim());
            }

            return fields.ToArray();
        }

        /// <summary>
        /// Determines each transaction's category from a list of available categories.
        /// </summary>
        /// <param name="transactions">Transaction list.</param>
        /// <param name="categories">Category list.</param>
        public void UpdateTransactionCategories(List<Transaction> transactions, List<Category> categories)
        {
            foreach (var transaction in transactions)
            {
                bool isSet = false;
                foreach (var category in categories)
                {
                    if ((!string.IsNullOrEmpty(category.OriginRegex) && Regex.IsMatch(transaction.Origin, category.OriginRegex))
                        || (!string.IsNullOrEmpty(category.DescriptionRegex) && Regex.IsMatch(transaction.Description, category.DescriptionRegex)))
                    {
                        transaction.Category = category.Name;
                        isSet = true;
                        break;
                    }
                }

                if (!isSet)
                {
                    transaction.Category = transaction.IsCredit ? "Other Income" : "Other Expense";
                }
            }
        }
    }
}
