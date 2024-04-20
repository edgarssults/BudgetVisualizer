using Ed.BudgetVisualizer.Models;
using System;
using System.Globalization;
using System.Linq;

namespace Ed.BudgetVisualizer.Logic.Parsers
{
    /// <summary>
    /// Swedbank transaction history parsing implementation.
    /// </summary>
    public class SwedbankParser : IParser
    {
        private readonly CultureInfo _culture = new CultureInfo("lv-LV");

        /// <summary>
        /// Whether the transaction history has a title row.
        /// </summary>
        public bool HasTitleRow => false;

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
        public static string FirstLineRegex => @"""Klienta konts"";""Ieraksta tips"";""Datums""";

        private readonly string[] _IgnoredDescriptions = new string[]
        {
            "Sākuma atlikums",
            "Apgrozījums",
            "Beigu atlikums",
        };

        /// <summary>
        /// Maps data fields to a transaction.
        /// </summary>
        /// <param name="fields">Data fields to map.</param>
        public Transaction MapTransaction(string[] fields)
        {
            if (_IgnoredDescriptions.Contains(fields[4]))
            {
                return null;
            }

            var transaction = new Transaction
            {
                Date = DateTime.Parse(fields[2], _culture),
                Origin = fields[3],
                Description = fields[4],
                IsDebit = fields[7] == "D",
                IsCredit = fields[7] == "K",
                Sum = decimal.Parse(fields[5].Replace(',', '.')),
                Currency = fields[6],
            };
            transaction.CategoryId = transaction.IsCredit ? 101 : 102; // TODO: Enum

            return transaction;
        }
    }
}
