using Ed.BudgetVisualizer.Logic.Parsers;
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
    /// Transaction history parsing logic.
    /// </summary>
    public class ParserLogic
    {
        /// <summary>
        /// Parses a file data stream and outputs a list of transactions.
        /// </summary>
        /// <param name="data">File data stream.</param>
        public async Task<List<Transaction>> ParseFile(Stream data)
        {
            // TODO: Exception feedback to UI

            IParser parser = null;
            var transactions = new List<Transaction>();
            int lineNumber = 0;
            string line;

            using (StreamReader reader = new StreamReader(data))
            {
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;

                    // TODO: Make parser selection nicer
                    if (lineNumber == 1)
                    {
                        if (Regex.IsMatch(line, SebParser.FirstLineRegex))
                        {
                            parser = new SebParser();
                        }
                        else if (Regex.IsMatch(line, SwedbankParser.FirstLineRegex))
                        {
                            parser = new SwedbankParser();
                        }
                        else
                        {
                            throw new NotSupportedException("CSV format not recognized and not supported.");
                        }
                    }

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (parser.HasTitleRow && lineNumber == 1)
                    {
                        continue;
                    }

                    if (parser.HasHeaderRow && lineNumber == 1)
                    {
                        continue;
                    }

                    if (parser.HasTitleRow && parser.HasHeaderRow && lineNumber == 2)
                    {
                        continue;
                    }

                    try
                    {
                        string[] fields = SplitAndClean(line, parser.Separator);
                        var transaction = parser.MapTransaction(fields);
                        if (transaction != null)
                        {
                            transactions.Add(transaction);
                        }
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
        /// Determines each transaction's category from a list of available categories.
        /// </summary>
        /// <param name="transactions">Transaction list.</param>
        /// <param name="categories">Category list.</param>
        public static void UpdateTransactionCategories(List<Transaction> transactions, List<Category> categories)
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

        /// <summary>
        /// Splits a line into fields and cleans each field from surrounding double quotes.
        /// </summary>
        /// <param name="line">Line to split and clean.</param>
        /// <param name="separator">Separator character.</param>
        private string[] SplitAndClean(string line, char separator)
        {
            var fields = new List<string>();

            // Match either fields surrounded in quotes or fields without quotes
            // Both fields end with the separator
            foreach (Match match in Regex.Matches(line, $"(?:\"(.*?)\"|(\\d+.*?)){separator}"))
            {
                // Add whichever group matched
                fields.Add(!string.IsNullOrEmpty(match.Groups[1].ToString())
                    ? match.Groups[1].ToString().Trim()
                    : match.Groups[2].ToString().Trim());
            }

            return fields.ToArray();
        }
    }
}
