using Ed.BudgetVisualizer.Logic;
using Ed.BudgetVisualizer.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Ed.BudgetVisualizer.Test
{
    public class ExtensionTests
    {
        [Fact]
        public void ToDiagramModel_Works()
        {
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    IsCredit = true,
                    IsDebit = false,
                    Currency = "EUR",
                    IsExcluded = false,
                    Sum = 100,
                    Origin = "Test",
                    Description = "Test",
                    Date = new DateTime(2019, 10, 1),
                    Category = "Category 1",
                },
                new Transaction
                {
                    IsCredit = true,
                    IsDebit = false,
                    Currency = "EUR",
                    IsExcluded = false,
                    Sum = 100,
                    Origin = "Test",
                    Description = "Test",
                    Date = new DateTime(2019, 11, 1),
                    Category = "Category 1",
                },
                new Transaction
                {
                    IsCredit = true,
                    IsDebit = false,
                    Currency = "EUR",
                    IsExcluded = false,
                    Sum = 100,
                    Origin = "Test",
                    Description = "Test",
                    Date = new DateTime(2019, 10, 2),
                    Category = "Category 2",
                },
            };

            DiagramViewModel result = transactions.ToDiagramModel();

            result.Diagrams.Count.Should().Be(2);
        }
    }
}
