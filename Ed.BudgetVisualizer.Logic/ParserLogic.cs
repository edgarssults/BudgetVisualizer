﻿using Ed.BudgetVisualizer.Logic.Parsers;
using Ed.BudgetVisualizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
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
        /// Prepares a CSV string from transactions.
        /// </summary>
        /// <param name="transactions">Transactions to include.</param>
        public string ToCsv(List<Transaction> transactions)
        {
            var nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            var sb = new StringBuilder();
            sb.AppendLine("\"Description\",\"Origin\",\"Sum\",\"IsDebit\",\"IsCredit\",\"CategoryId\"");

            foreach (var t in transactions)
            {
                sb.AppendLine($"\"{Clean(t.Description)}\",\"{Clean(t.Origin)}\",{t.Sum.ToString(nfi)},\"{t.IsDebit}\",\"{t.IsCredit}\",{t.CategoryId}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines each transaction's category from a list of available category matches.
        /// </summary>
        /// <param name="transactions">Transaction list.</param>
        public void MatchCategories(List<Transaction> transactions)
        {
            var matches = GetMatches();

            foreach (var transaction in transactions)
            {
                bool isSet = false;
                foreach (var match in matches)
                {
                    if ((!string.IsNullOrEmpty(match.OriginRegex) && Regex.IsMatch(transaction.Origin, match.OriginRegex))
                        || (!string.IsNullOrEmpty(match.DescriptionRegex) && Regex.IsMatch(transaction.Description, match.DescriptionRegex)))
                    {
                        transaction.CategoryId = match.CategoryId;
                        isSet = true;
                        break;
                    }
                }

                if (!isSet)
                {
                    transaction.CategoryId = transaction.IsCredit ? 101 : 102; // TODO: Enum
                }
            }
        }

        /// <summary>
        /// Determines each transaction's category from a list of available category matches.
        /// </summary>
        /// <param name="transactions">Transaction list.</param>
        public void PredictCategories(List<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                var prediction = BudgetVisualizer.Predict(new BudgetVisualizer.ModelInput
                {
                    Description = transaction.Description,
                    Origin = transaction.Origin,
                    IsCredit = transaction.IsCredit,
                    IsDebit = transaction.IsDebit
                });

                // TODO: Check score of prediction
                transaction.CategoryId = (int)prediction.PredictedLabel;
            }
        }

        public List<Models.Match> GetMatches()
        {
            var assembly = typeof(ParserLogic).Assembly;
            var modelStream = assembly.GetManifestResourceStream($"Ed.BudgetVisualizer.Logic.Data.matches.json");
            using var streamReader = new StreamReader(modelStream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var jsonSerializer = new JsonSerializer();
            return jsonSerializer.Deserialize<List<Models.Match>>(jsonTextReader);
        }

        public List<Category> GetCategories()
        {
            var assembly = typeof(ParserLogic).Assembly;
            var modelStream = assembly.GetManifestResourceStream($"Ed.BudgetVisualizer.Logic.Data.categories.json");
            using var streamReader = new StreamReader(modelStream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var jsonSerializer = new JsonSerializer();
            return jsonSerializer.Deserialize<List<Models.Category>>(jsonTextReader);
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
            foreach (System.Text.RegularExpressions.Match match in Regex.Matches(line, $"(?:\"(.*?)\"|(\\d+.*?)){separator}"))
            {
                // Add whichever group matched
                fields.Add(!string.IsNullOrEmpty(match.Groups[1].ToString())
                    ? match.Groups[1].ToString().Trim()
                    : match.Groups[2].ToString().Trim());
            }

            return fields.ToArray();
        }

        private string Clean(string input)
        {
            return input.Replace("\"", "");
        }
    }
}
