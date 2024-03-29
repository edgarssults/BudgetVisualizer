using Ed.BudgetVisualizer.Models;
using System;
using System.Globalization;

namespace Ed.BudgetVisualizer.Logic.Parsers
{
    /// <summary>
    /// SEB transaction history parsing implementation.
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
        /// Regular expression pattern for the first line of the CSV file to identify the correct parser.
        /// </summary>
        public static string FirstLineRegex => @"""KONTAA\s*\(LV[^\)]+\)\s*PĀRSKATS";

        /// <summary>
        /// Maps data fields to a transaction.
        /// </summary>
        /// <param name="fields">Data fields to map.</param>
        public Transaction MapTransaction(string[] fields) 
        {
            var lv = new CultureInfo("lv-LV");
            var transaction = new Transaction
            {
                Date = DateTime.Parse(fields[1], lv),
                Origin = fields[4],
                Description = fields[9],
                IsDebit = fields[14] == "D",
                IsCredit = fields[14] == "C",
                Sum = decimal.Parse(fields[15]),
                Currency = fields[17],
            };
            transaction.CategoryId = transaction.IsCredit ? 101 : 102; // TODO: Enum
            return transaction;
        }
    }
}
